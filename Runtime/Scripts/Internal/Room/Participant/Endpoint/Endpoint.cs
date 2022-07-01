// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections;
using UnityEngine;

namespace Sendbird.Calls
{
    internal class Endpoint : PeerConnectionClientEventListener
    {
        private static readonly WaitForSecondsRealtime WAIT_FOR_RECONNECT_INTERVAL = new WaitForSecondsRealtime(5.0f);

        private readonly SbRoom _ownedRoom = null;

        internal PeerConnectionClient PeerConnectionClient { get; private set; } = null;
        private string _endpointId = null;
        private EndpointState _endpointState;
        private Coroutine _reconnectCoroutine = null;
        
        private readonly EndpointType _audioEndpointType;
        private readonly EndpointType _videoEndpointType;

        internal Endpoint(SbRoom ownedRoom, EndpointType audioEndpointType, 
                          EndpointType videoEndpointType, bool isAudioEnabled, bool isVideoEnabled)
        {
            _ownedRoom = ownedRoom;
            _audioEndpointType = audioEndpointType;
            _videoEndpointType = videoEndpointType;
            _endpointState = EndpointState.Idle;
            CreatePeerConnectionClient(_audioEndpointType, isAudioEnabled, isVideoEnabled);
        }

        internal void Connect()
        {
            CreateOffer();
        }

        internal void Close(bool shouldDeleteEndpoint)
        {
            _endpointState = EndpointState.Closing;

            if (_reconnectCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_reconnectCoroutine);
                _reconnectCoroutine = null;
            }
            
            if (shouldDeleteEndpoint)
            {
                SendDeleteEndpoint();
            }

            PeerConnectionClient.Close();
            PeerConnectionClient = null;
        }
        
        private void CreatePeerConnectionClient(EndpointType audioEndpointType, bool isAudioEnabled, bool isVideoEnabled)
        {
            PeerConnectionClient = new PeerConnectionClient(this);
            PeerConnectionClient.Init(audioEndpointType);
            PeerConnectionClient.TrySetAudioEnable(isAudioEnabled);
            PeerConnectionClient.TrySetVideoEnable(isVideoEnabled);
        }
        
        private void RenewPeerConnectionClient()
        {
            bool isAudioEnabled = false;
            bool isVideoEnabled = false;
            
            if (PeerConnectionClient != null)
            {
                isAudioEnabled = PeerConnectionClient.IsAudioEnabled;
                isVideoEnabled = PeerConnectionClient.IsVideoEnabled;
                PeerConnectionClient.Close();
                PeerConnectionClient = null;
            }

            CreatePeerConnectionClient(_audioEndpointType, isAudioEnabled, isVideoEnabled);
        }

        private void ReconnectAfterInterval(string reasonDescription)
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"Endpoint::ReconnectAfterInterval {reasonDescription}");

            if (_reconnectCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_reconnectCoroutine);
                _reconnectCoroutine = null;
            }

            _reconnectCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(ReconnectAfterIntervalCoroutine());
        }

        private IEnumerator ReconnectAfterIntervalCoroutine()
        {
            RenewPeerConnectionClient();

            yield return WAIT_FOR_RECONNECT_INTERVAL;

            CreateOffer();
        }

        private void CreateOffer()
        {
            _endpointState = EndpointState.Offering;
            PeerConnectionClient.CreateOffer();
        }

        private void SendCreateOrUpdateEndpoint(string sdp)
        {
            void OnCreateOrUpdateEndpointResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (PeerConnectionClient == null || _endpointState == EndpointState.Closing || _endpointState == EndpointState.Closed)
                {
                    Logger.LogInfo(Logger.CategoryType.Rtc, $"Endpoint::SendCreateOrUpdateEndpoint::OnCreateOrUpdateEndpointResultHandler invalid state:{_endpointState}");
                    return;
                }
                
                if (error == null)
                {
                    _endpointState = EndpointState.Connecting;
                    if (apiResponse is CreateEndpointApiCommand.Response createEndpointResponse)
                    {
                        _endpointId = createEndpointResponse.endpointId;
                        PeerConnectionClient.SetRemoteDescription(createEndpointResponse.sdp);
                    }
                    else if (apiResponse is UpdateEndpointApiCommand.Response updateEndpointResponse)
                    {
                        _endpointId = updateEndpointResponse.endpointId;
                        PeerConnectionClient.SetRemoteDescription(updateEndpointResponse.sdp);
                    }
                }
                else
                {
                    ReconnectAfterInterval($"Failed CreateOrUpdateEndpoint ErrorMessage:{error.ErrorMessage}");
                }
            }

            string ownedRoomId = _ownedRoom?.RoomId;
            string localParticipantId = _ownedRoom?.LocalParticipant?.ParticipantId;

            if (string.IsNullOrEmpty(ownedRoomId) || string.IsNullOrEmpty(localParticipantId))
            {
                ReconnectAfterInterval($"Failed CreateOrUpdateEndpoint invalid roomId:{ownedRoomId} or localParticipantId:{localParticipantId}");
                return;
            }

            if (string.IsNullOrEmpty(_endpointId))
            {
                CreateEndpointApiCommand.Request createEndpointRequest = new CreateEndpointApiCommand.Request(
                    ownedRoomId, localParticipantId, sdp, _audioEndpointType, _videoEndpointType, OnCreateOrUpdateEndpointResultHandler);

                CommandRouter.Instance.Send(createEndpointRequest);
            }
            else
            {
                UpdateEndpointApiCommand.Request updateEndpointRequest = new UpdateEndpointApiCommand.Request(
                    ownedRoomId, localParticipantId, _endpointId, sdp, OnCreateOrUpdateEndpointResultHandler);

                CommandRouter.Instance.Send(updateEndpointRequest);
            }
        }

        private void SendRoomConnected()
        {
            if (_audioEndpointType.IsSendAble() == false && _videoEndpointType.IsSendAble() == false)
            {
                return;
            }
            
            void OnResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error != null)
                {
                    ReconnectAfterInterval($"Failed RoomConnected ErrorMessage:{error.ErrorMessage}");
                }
            }
            
            string ownedRoomId = _ownedRoom?.RoomId;
            string localParticipantId = _ownedRoom?.LocalParticipant?.ParticipantId;
            
            if (string.IsNullOrEmpty(ownedRoomId) || string.IsNullOrEmpty(localParticipantId))
            {
                ReconnectAfterInterval($"Failed RoomConnected invalid roomId:{ownedRoomId} or localParticipantId:{localParticipantId}");
                return;
            }
            
            RoomConnectedApiCommand.Request roomConnectedRequest = new RoomConnectedApiCommand.Request(ownedRoomId, localParticipantId, _endpointId, OnResultHandler);
            CommandRouter.Instance.Send(roomConnectedRequest);
        }
        
        private void SendDeleteEndpoint()
        {
            void OnResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error != null)
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"SendDeleteEndpoint ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
                }
                else
                {
                    _endpointId = null;
                }
            }
            
            string ownedRoomId = _ownedRoom?.RoomId;
            string localParticipantId = _ownedRoom?.LocalParticipant?.ParticipantId;
            
            if (string.IsNullOrEmpty(ownedRoomId) || string.IsNullOrEmpty(localParticipantId) || string.IsNullOrEmpty(_endpointId))
            {
                ReconnectAfterInterval($"Failed DeleteEndpoint invalid roomId:{ownedRoomId} or localParticipantId:{localParticipantId}");
                return;
            }
            
            DeleteEndpointApiCommand.Request deleteEndpointRequest = new DeleteEndpointApiCommand.Request(ownedRoomId, localParticipantId, _endpointId, OnResultHandler);
            CommandRouter.Instance.Send(deleteEndpointRequest);
        }

        void PeerConnectionClientEventListener.OnRTCSetLocalDescription(string sdp)
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"Endpoint::OnRTCSetLocalDescription sdp:{sdp}");
            
            SendCreateOrUpdateEndpoint(sdp);
        }

        void PeerConnectionClientEventListener.OnPeerConnectionClientError(string errorMessage)
        {
            switch (_endpointState)
            {
                case EndpointState.Offering:
                case EndpointState.Connecting:
                case EndpointState.Connected:
                {
                    ReconnectAfterInterval(errorMessage);
                    break;
                }
                default:
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"Endpoint::OnPeerConnectionClientError error:{errorMessage}");
                    break;
                }
            }
        }

        void PeerConnectionClientEventListener.OnPeerConnectionClientConnected()
        {
            switch (_endpointState)
            {
                case EndpointState.Connecting:
                {
                    _endpointState = EndpointState.Connected;
                    SendRoomConnected();
                    break;
                }
                default:
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"Endpoint::OnPeerConnectionClientConnected CurrentState:{_endpointState}");
                    break;
                }
            }
        }

        void PeerConnectionClientEventListener.OnPeerConnectionClientReconnectionFailed()
        {
            if (_endpointState == EndpointState.Connected)
            {
                ReconnectAfterInterval($"OnPeerConnectionClientClosed CurrentState:{_endpointState}");
            }
        }

        void PeerConnectionClientEventListener.OnPeerConnectionClientClosed()
        {
            switch (_endpointState)
            {
                case EndpointState.Connecting:
                {
                    ReconnectAfterInterval($"OnPeerConnectionClientClosed CurrentState:{_endpointState}");
                    break;
                }
                case EndpointState.Closing:
                {
                    Logger.LogInfo(Logger.CategoryType.Rtc, $"OnPeerConnectionClientClosed CurrentState:{_endpointState}");
                    _endpointState = EndpointState.Closed;
                    break;
                }
                default:
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"OnPeerConnectionClientClosed CurrentState:{_endpointState}");
                    break;
                }
            }
        }
    }
}