// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections;
using Unity.WebRTC;
using UnityEngine;

namespace Sendbird.Calls
{
    internal class PeerConnectionClient
    {
        private const int AUDIO_FREQUENCY = 48000;

        private readonly PeerConnectionClientEventListener _peerConnectionClientEventListener = null;

        private PeerConnectionClientState _state;
        private AudioSource _outputAudioSource = null;
        private AudioSource _inputAudioSource = null;

        private RTCPeerConnection _rtcPeerConnection = null;

        private RTCRtpTransceiver _rtcAudioTransceiver = null;
        private AudioStreamTrack _sendAudioStreamTrack = null;
        private AudioStreamTrack _receiveAudioStreamTrack = null;

        internal bool IsAudioEnabled { get; private set; } = false;
        internal bool IsVideoEnabled { get; private set; } = false;

        private string _playedMicrophoneDeviceName = null;
        private Coroutine _createSendAudioStreamTrackCoroutine = null;
        private Coroutine _createReceiveAudioStreamTrackCoroutine = null;
        private Coroutine _changeAudioInputMicrophoneDeviceCoroutine = null;

        internal PeerConnectionClient(PeerConnectionClientEventListener peerConnectionClientEventListener)
        {
            _peerConnectionClientEventListener = peerConnectionClientEventListener;
        }

        internal void Init(EndpointType audioEndpointType)
        {
            _state = PeerConnectionClientState.Idle;

            RTCConfiguration configuration = default;

            _rtcPeerConnection = new RTCPeerConnection(ref configuration);

            _rtcPeerConnection.OnIceCandidate = OnIceCandidateHandler;
            _rtcPeerConnection.OnIceConnectionChange = OnIceConnectionChangeHandler;

            _rtcAudioTransceiver = _rtcPeerConnection.AddTransceiver(TrackKind.Audio);
            _rtcAudioTransceiver.Direction = audioEndpointType.ToTransceiverDirection();
        }

        internal void Close()
        {
            _state = PeerConnectionClientState.Closed;

            AudioSettings.OnAudioConfigurationChanged -= AudioConfigurationChangeHandler;
            
            if (_changeAudioInputMicrophoneDeviceCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_changeAudioInputMicrophoneDeviceCoroutine);
                _changeAudioInputMicrophoneDeviceCoroutine = null;
            }

            if (_createSendAudioStreamTrackCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_createSendAudioStreamTrackCoroutine);
                _createSendAudioStreamTrackCoroutine = null;
            }

            if (_createReceiveAudioStreamTrackCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_createReceiveAudioStreamTrackCoroutine);
                _createReceiveAudioStreamTrackCoroutine = null;
            }

            if (string.IsNullOrEmpty(_playedMicrophoneDeviceName) == false)
            {
                Microphone.End(_playedMicrophoneDeviceName);
                _playedMicrophoneDeviceName = null;
            }

            if (_inputAudioSource != null)
            {
                _inputAudioSource.clip = null;
                _inputAudioSource.Stop();
                Object.Destroy(_inputAudioSource.gameObject);
                _inputAudioSource = null;
            }

            if (_outputAudioSource != null)
            {
                _outputAudioSource.clip = null;
                _outputAudioSource.Stop();
                Object.Destroy(_outputAudioSource.gameObject);
                _outputAudioSource = null;
            }

            _sendAudioStreamTrack?.Dispose();
            _sendAudioStreamTrack = null;

            _receiveAudioStreamTrack?.Dispose();
            _receiveAudioStreamTrack = null;

            _rtcAudioTransceiver?.Dispose();
            _rtcAudioTransceiver = null;

            _rtcPeerConnection?.Dispose();
            _rtcPeerConnection = null;

            _peerConnectionClientEventListener?.OnPeerConnectionClientClosed();
        }

        internal void CreateOffer()
        {
            _state = PeerConnectionClientState.Offering;
            SendbirdCallGameObject.Instance.StartCoroutine(CreateOfferCoroutine());
            CreateAudioStreamTrack();
        }

        private void CreateAudioStreamTrack()
        {
            if (_createSendAudioStreamTrackCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_createSendAudioStreamTrackCoroutine);
                _createSendAudioStreamTrackCoroutine = null;
            }

            if (_createReceiveAudioStreamTrackCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_createReceiveAudioStreamTrackCoroutine);
                _createReceiveAudioStreamTrackCoroutine = null;
            }

            _createSendAudioStreamTrackCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(CreateSendAudioStreamTrackCoroutine());
            _createReceiveAudioStreamTrackCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(CreateReceiveAudioStreamTrackCoroutine());
        }

        private IEnumerator CreateSendAudioStreamTrackCoroutine()
        {
            if (_rtcAudioTransceiver.Direction != RTCRtpTransceiverDirection.SendOnly && _rtcAudioTransceiver.Direction != RTCRtpTransceiverDirection.SendRecv)
            {
                yield break;
            }

            Logger.LogInfo(Logger.CategoryType.Rtc, $"CreateSendAudioStreamTrack start");

            if (Microphone.devices.Length <= 0)
            {
                Logger.LogWarning(Logger.CategoryType.Rtc, $"CreateSendAudioStreamTrack microphone devices is empty");
                yield break;
            }

            if (_inputAudioSource == null)
            {
                GameObject audioInputGameObject = new GameObject("AudioInput")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _inputAudioSource = audioInputGameObject.AddComponent<AudioSource>();
            }

            if (_changeAudioInputMicrophoneDeviceCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_changeAudioInputMicrophoneDeviceCoroutine);
                _changeAudioInputMicrophoneDeviceCoroutine = null;
            }

            _changeAudioInputMicrophoneDeviceCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(ChangeAudioInputMicrophoneDevice());

            if (_sendAudioStreamTrack == null)
            {
                _sendAudioStreamTrack = new AudioStreamTrack(_inputAudioSource);
                _sendAudioStreamTrack.Loopback = false;
            }
            else
            {
                _inputAudioSource.SetTrack(_sendAudioStreamTrack);
            }
            
            _sendAudioStreamTrack.Enabled = IsAudioEnabled;

            _rtcAudioTransceiver?.Sender.ReplaceTrack(_sendAudioStreamTrack);

            AudioSettings.OnAudioConfigurationChanged -= AudioConfigurationChangeHandler;
            AudioSettings.OnAudioConfigurationChanged += AudioConfigurationChangeHandler;

            _createSendAudioStreamTrackCoroutine = null;
        }

        private void AudioConfigurationChangeHandler(bool deviceWasChanged)
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"AudioConfigurationChangeHandler deviceWasChanged:{deviceWasChanged}");

            if (deviceWasChanged)
            {
                if (_changeAudioInputMicrophoneDeviceCoroutine != null)
                {
                    SendbirdCallGameObject.Instance.StopCoroutine(_changeAudioInputMicrophoneDeviceCoroutine);
                    _changeAudioInputMicrophoneDeviceCoroutine = null;
                }

                _changeAudioInputMicrophoneDeviceCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(ChangeAudioInputMicrophoneDevice());
            }

            if (_inputAudioSource != null)
            {
                _inputAudioSource.Stop();
                _inputAudioSource.Play();
            }
            
            if (_outputAudioSource != null)
            {
                _outputAudioSource.Stop();
                _outputAudioSource.Play();
            }
        }

        private IEnumerator ChangeAudioInputMicrophoneDevice()
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"ChangeAudioInputMicrophoneDevice PrevDevice:{_playedMicrophoneDeviceName}");

            if (string.IsNullOrEmpty(_playedMicrophoneDeviceName) == false)
            {
                Logger.LogInfo(Logger.CategoryType.Rtc, $"ChangeAudioInputMicrophoneDevice End Microphone Device:{_playedMicrophoneDeviceName}");
                Microphone.End(_playedMicrophoneDeviceName);
                _playedMicrophoneDeviceName = null;
            }
            
            if (Microphone.devices.Length <= 0 || _inputAudioSource == null)
            {
                Logger.LogWarning(Logger.CategoryType.Rtc, $"ChangeAudioInputMicrophoneDevice Failed because invalid devices");
                yield break;
            }

            _inputAudioSource.Stop();
            _inputAudioSource.clip = null;
            
            _playedMicrophoneDeviceName = Microphone.devices[0];
            AudioClip audioClip = Microphone.Start(_playedMicrophoneDeviceName, true, 1, AUDIO_FREQUENCY);

            while (Microphone.GetPosition(_playedMicrophoneDeviceName) <= 0)
            {
                yield return null;
            }

            _inputAudioSource.clip = audioClip;
            _inputAudioSource.loop = true;
            _inputAudioSource.volume = 1f;
            _inputAudioSource.Play();

            _changeAudioInputMicrophoneDeviceCoroutine = null;

            Logger.LogInfo(Logger.CategoryType.Rtc, $"ChangeAudioInputMicrophoneDevice PlayDevice:{_playedMicrophoneDeviceName}");
        }

        private IEnumerator CreateReceiveAudioStreamTrackCoroutine()
        {
            if (_rtcAudioTransceiver.Direction != RTCRtpTransceiverDirection.RecvOnly && _rtcAudioTransceiver.Direction != RTCRtpTransceiverDirection.SendRecv)
                yield break;

            Logger.LogInfo(Logger.CategoryType.Rtc, $"CreateReceiveAudioStreamTrack start");

            while (_rtcAudioTransceiver == null)
            {
                yield return null;
            }

            if (_outputAudioSource == null)
            {
                GameObject audioOutputGameObject = new GameObject("AudioOutput")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _outputAudioSource = audioOutputGameObject.AddComponent<AudioSource>();
            }

            _outputAudioSource.loop = true;
            _outputAudioSource.volume = 1f;
            _outputAudioSource.Play();

            if (_rtcAudioTransceiver.Receiver.Track is AudioStreamTrack receiverAudioStreamTrack)
            {
                _outputAudioSource.SetTrack(receiverAudioStreamTrack);
            }
            else
            {
                Logger.LogWarning(Logger.CategoryType.Rtc, $"CreateReceiveAudioStreamTrack receiver track is not audio stream");
            }

            _createReceiveAudioStreamTrackCoroutine = null;
        }

        private IEnumerator CreateOfferCoroutine()
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"CreateOffer start");

            RTCSessionDescriptionAsyncOperation sessionDescriptionAsync = _rtcPeerConnection.CreateOffer();
            yield return sessionDescriptionAsync;

            if (!sessionDescriptionAsync.IsError)
            {
                if (_rtcPeerConnection.SignalingState != RTCSignalingState.Stable)
                {
                    string errorMessage = $"CreateOffer Signaling state is not stable.";
                    _peerConnectionClientEventListener.OnPeerConnectionClientError(errorMessage);
                    yield break;
                }

                yield return SendbirdCallGameObject.Instance.StartCoroutine(SetLocalDescriptionCoroutine(sessionDescriptionAsync.Desc));
            }
            else
            {
                string errorMessage = $"CreateOffer Error:{sessionDescriptionAsync.Error}";
                _peerConnectionClientEventListener.OnPeerConnectionClientError(errorMessage);
            }
        }

        private IEnumerator SetLocalDescriptionCoroutine(RTCSessionDescription rtcSessionDescription)
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"SetLocalDescription start");

            RTCSetSessionDescriptionAsyncOperation localDescriptionAsync = _rtcPeerConnection.SetLocalDescription(ref rtcSessionDescription);
            yield return localDescriptionAsync;

            if (!localDescriptionAsync.IsError)
            {
                _peerConnectionClientEventListener.OnRTCSetLocalDescription(rtcSessionDescription.sdp);
            }
            else
            {
                string errorMessage = $"SetLocalDescription Error:{localDescriptionAsync.Error}";
                _peerConnectionClientEventListener.OnPeerConnectionClientError(errorMessage);
            }
        }

        public void SetRemoteDescription(string answerSdp)
        {
            SendbirdCallGameObject.Instance.StartCoroutine(SetRemoteDescriptionCoroutine(answerSdp));
        }

        private IEnumerator SetRemoteDescriptionCoroutine(string answerSdp)
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"PeerConnectionClient::SetRemoteDescription");

            if (_rtcPeerConnection == null || _state == PeerConnectionClientState.Closed)
            {
                Logger.LogInfo(Logger.CategoryType.Rtc, $"PeerConnectionClient::SetRemoteDescription RtcPeerConnection is null or state type is invalid:{_state}");
                yield break;
            }

            RTCSessionDescription remoteSessionDescription = new RTCSessionDescription() { type = RTCSdpType.Answer, sdp = answerSdp, };
            RTCSetSessionDescriptionAsyncOperation remoteDescriptionAsync = _rtcPeerConnection.SetRemoteDescription(ref remoteSessionDescription);

            yield return remoteDescriptionAsync;

            if (remoteDescriptionAsync.IsError)
            {
                string errorMessage = $"Failed remote session description Error:{remoteDescriptionAsync.Error}";
                _peerConnectionClientEventListener.OnPeerConnectionClientError(errorMessage);
            }
        }

        private void OnIceCandidateHandler(RTCIceCandidate iceCandidate)
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"OnIceCandidateHandler:{iceCandidate.Candidate}");
        }

        private void OnIceConnectionChangeHandler(RTCIceConnectionState rtcIceConnectionState)
        {
            Logger.LogInfo(Logger.CategoryType.Rtc, $"OnIceConnectionChangeHandler RTCIceConnectionState:{rtcIceConnectionState} CurrentState:{_state}");

            if (rtcIceConnectionState == RTCIceConnectionState.Connected)
            {
                if (_state == PeerConnectionClientState.Answering || _state == PeerConnectionClientState.RestartAnswering ||
                    _state == PeerConnectionClientState.Offering || _state == PeerConnectionClientState.RestartOffering ||
                    _state == PeerConnectionClientState.Reconnecting)
                {
                    _state = PeerConnectionClientState.Connected;
                    _peerConnectionClientEventListener.OnPeerConnectionClientConnected();
                }
                else
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"OnIceConnectionChangeHandler RTCIceConnectionState:{rtcIceConnectionState} Ignore because CurrentState:{_state}");
                }
            }
            else if (rtcIceConnectionState == RTCIceConnectionState.Disconnected)
            {
                if (_state == PeerConnectionClientState.Connected)
                {
                    _state = PeerConnectionClientState.Reconnecting;
                }
                else
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"OnIceConnectionChangeHandler RTCIceConnectionState:{rtcIceConnectionState} Ignore because CurrentState:{_state}");
                }
            }
            else if (rtcIceConnectionState == RTCIceConnectionState.Failed)
            {
                if (_state == PeerConnectionClientState.Answering || _state == PeerConnectionClientState.Offering)
                {
                    Close();
                }
                else if (_state == PeerConnectionClientState.Reconnecting)
                {
                    _state = PeerConnectionClientState.ReconnectionFailed;
                    _peerConnectionClientEventListener.OnPeerConnectionClientReconnectionFailed();
                }
                else
                {
                    Logger.LogWarning(Logger.CategoryType.Rtc, $"OnIceConnectionChangeHandler RTCIceConnectionState:{rtcIceConnectionState} Ignore because CurrentState:{_state}");
                }
            }
        }

        internal bool TrySetAudioEnable(bool enable)
        {
            if (IsAudioEnabled == enable) return false;

            IsAudioEnabled = enable;

            if (_sendAudioStreamTrack != null)
            {
                _sendAudioStreamTrack.Enabled = IsAudioEnabled;
            }

            return true;
        }

        internal bool TrySetVideoEnable(bool enable)
        {
            IsVideoEnabled = enable;
            return true;
        }
    }
}