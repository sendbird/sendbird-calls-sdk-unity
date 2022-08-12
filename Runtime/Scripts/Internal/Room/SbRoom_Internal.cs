// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using UnityEngine;

namespace Sendbird.Calls
{
    public partial class SbRoom
    {
        private static readonly ReadOnlyDictionary<string, string> EMPTY_READ_ONLY_CUSTOM_ITEMS = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(0));
        private static readonly ReadOnlyCollection<string> EMPTY_READ_ONLY_CUSTOM_ITEM_KEYS = new ReadOnlyCollection<string>(new List<string>(0));
        private readonly RoomParticipantCollection _participantCollection = null;
        private ParticipantManagerAbstract _participantManager = null;

        private readonly List<SbRoomEventListener> _roomEventListeners = new List<SbRoomEventListener>();
        private Coroutine _aliveCoroutine;
        private int _aliveFailureCount;
        private long _customItemsUpdateAt;
        private bool _isEntering = false;

        internal SbRoom(RoomCommandObject roomCommandObject)
        {
            _participantCollection = new RoomParticipantCollection();

            RoomId = roomCommandObject.roomId;
            CreatedAt = roomCommandObject.createdAt;
            CreatedBy = roomCommandObject.createdBy;
            RoomType = SbRoomTypeExtension.JsonPropertyNameToType(roomCommandObject.roomType);
            State = SbRoomStateExtension.JsonPropertyNameToType(roomCommandObject.state);
            _isEntering = false;

            _participantCollection.ClearAllParticipants();
            if (roomCommandObject.currentParticipantCommandObjects != null && 0 < roomCommandObject.currentParticipantCommandObjects.Count)
            {
                foreach (ParticipantCommandObject participantCommandObject in roomCommandObject.currentParticipantCommandObjects)
                {
                    _participantCollection.InsertRemoteParticipant(participantCommandObject);
                }
            }

            _customItemsUpdateAt = long.MinValue;
            CustomItems = roomCommandObject.customItems;
        }

        private void Enter_Internal(SbRoomEnterParams roomEnterParams, SbCompletionHandler completionHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"SbRoom::Enter RoomId:{RoomId}");

            if (roomEnterParams == null)
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("RoomEnterParams");
                Logger.LogWarning(Logger.CategoryType.Room, $"SbRoom::Enter {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => completionHandler?.Invoke(error));
                return;
            }

            if (_isEntering)
            {
                SbError error = new SbError(SbErrorCode.EnteringRoomStillInProgress);
                Logger.LogWarning(Logger.CategoryType.Room, $"SbRoom::Enter {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => completionHandler?.Invoke(error));
                return;
            }

            if (IsEntered() || RoomManager.Instance.HasEnteringOrEnteredRoom())
            {
                SbError error = new SbError(SbErrorCode.ParticipantAlreadyInRoom);
                Logger.LogWarning(Logger.CategoryType.Room, $"SbRoom::Enter {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => completionHandler?.Invoke(error));
                return;
            }

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                _isEntering = false;
                if (error == null)
                {
                    if (apiResponse is EnterRoomApiCommand.Response enterRoomResponse)
                    {
                        _participantCollection.CreateOrUpdateLocalParticipant(enterRoomResponse.meParticipantCommandObject);
                        UpdateFromRoomCommandObject(enterRoomResponse.roomCommandObject);

                        _participantManager = new AudioMcuParticipantManager(this);
                        _participantManager.OnLocalParticipantEntered(LocalParticipant);

                        completionHandler?.Invoke(null);
                        StartAliveCommand();
                    }
                    else
                    {
                        completionHandler?.Invoke(new SbError(SbErrorCode.MalformedData));
                    }
                }
                else
                {
                    completionHandler?.Invoke(error);
                }
            }

            _isEntering = true;
            EnterRoomApiCommand.Request request = new EnterRoomApiCommand.Request(RoomId, roomEnterParams.AudioEnabled, false, ResultHandler);
            CommandRouter.Instance.Send(request);

            CommandRouter.Instance.ConnectWebSocketAsync();
        }

        private void Exit_Internal()
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"SbRoom::Exit RoomId:{RoomId}");

            if (LocalParticipant == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"SbRoom::Exit The client has already exited the room.");
                return;
            }

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error != null)
                {
                    Logger.LogWarning(Logger.CategoryType.Room, $"SbRoom::Exit Failed exit request ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
                }
            }

            ExitRoomApiCommand.Request request = new ExitRoomApiCommand.Request(RoomId, LocalParticipant.ParticipantId, ResultHandler);
            CommandRouter.Instance.Send(request);
            OnLocalParticipantExited(null);
        }

        private void FetchCustomItems_Internal(SbRoomFetchCustomItemsHandler roomFetchCustomItemsHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"SbRoom::FetchCustomItems RoomId:{RoomId}");

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error == null)
                {
                    if (apiResponse is RoomFetchCustomItemsApiCommand.Response fetchCustomItemsResponse)
                    {
                        TryResetCustomItems(fetchCustomItemsResponse.customItems, fetchCustomItemsResponse.affectedAt);
                        roomFetchCustomItemsHandler?.Invoke(CustomItems, null);
                    }
                    else
                    {
                        roomFetchCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, new SbError(SbErrorCode.MalformedData));
                    }
                }
                else
                {
                    roomFetchCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, error);
                }
            }

            RoomFetchCustomItemsApiCommand.Request request = new RoomFetchCustomItemsApiCommand.Request(RoomId, ResultHandler);
            CommandRouter.Instance.Send(request);
        }

        private void UpdateCustomItems_Internal(Dictionary<string, string> customItems, SbRoomCustomItemsHandler roomCustomItemsHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"SbRoom::UpdateCustomItems RoomId:{RoomId} CustomItems:{JsonConvert.SerializeObject(customItems)}");

            if (customItems == null || customItems.Count <= 0)
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("CustomItems");
                Logger.LogWarning(Logger.CategoryType.Room, $"SbRoom::UpdateCustomItems {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => roomCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, EMPTY_READ_ONLY_CUSTOM_ITEM_KEYS, error));
                return;
            }

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error == null)
                {
                    if (apiResponse is RoomUpdateCustomItemsApiCommand.Response deleteCustomItemsResponse)
                    {
                        TryResetCustomItems(deleteCustomItemsResponse.customItems, deleteCustomItemsResponse.affectedAt);
                        roomCustomItemsHandler?.Invoke(CustomItems, deleteCustomItemsResponse.updatedKeys, null);
                    }
                    else
                    {
                        roomCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, EMPTY_READ_ONLY_CUSTOM_ITEM_KEYS, new SbError(SbErrorCode.MalformedData));
                    }
                }
                else
                {
                    roomCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, EMPTY_READ_ONLY_CUSTOM_ITEM_KEYS, error);
                }
            }

            RoomUpdateCustomItemsApiCommand.Request request = new RoomUpdateCustomItemsApiCommand.Request(RoomId, customItems, ResultHandler);
            CommandRouter.Instance.Send(request);
        }

        private void DeleteCustomItems_Internal(List<string> customKeys, SbRoomCustomItemsHandler roomCustomItemsHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"SbRoom::DeleteCustomItems RoomId:{RoomId} CustomKeys:{JsonConvert.SerializeObject(customKeys)}");

            if (customKeys == null || customKeys.Count <= 0)
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("CustomKeys");
                Logger.LogWarning(Logger.CategoryType.Room, $"SbRoom::DeleteCustomItems {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => roomCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, EMPTY_READ_ONLY_CUSTOM_ITEM_KEYS, error));
                return;
            }

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error == null)
                {
                    if (apiResponse is RoomDeleteCustomItemsApiCommand.Response deleteCustomItemsResponse)
                    {
                        TryResetCustomItems(deleteCustomItemsResponse.customItems, deleteCustomItemsResponse.affectedAt);
                        roomCustomItemsHandler?.Invoke(deleteCustomItemsResponse.customItems, deleteCustomItemsResponse.deletedKeys, null);
                    }
                    else
                    {
                        roomCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, EMPTY_READ_ONLY_CUSTOM_ITEM_KEYS, new SbError(SbErrorCode.MalformedData));
                    }
                }
                else
                {
                    roomCustomItemsHandler?.Invoke(EMPTY_READ_ONLY_CUSTOM_ITEMS, EMPTY_READ_ONLY_CUSTOM_ITEM_KEYS, error);
                }
            }

            RoomDeleteCustomItemsApiCommand.Request request = new RoomDeleteCustomItemsApiCommand.Request(RoomId, customKeys, ResultHandler);
            CommandRouter.Instance.Send(request);
        }

        private void UpdateFromRoomCommandObject(RoomCommandObject roomCommandObject)
        {
            SbRoomState newState = SbRoomStateExtension.JsonPropertyNameToType(roomCommandObject.state);
            if (State == SbRoomState.Open && newState == SbRoomState.Deleted)
            {
                OnRoomDeleted();
            }

            State = newState;

            //Temporary setting for exit check
            _participantCollection.ForceChangeExitedStateToAllRemoteParticipants();

            if (roomCommandObject.currentParticipantCommandObjects != null)
            {
                SbLocalParticipant localParticipant = _participantCollection.GetLocalParticipant();
                foreach (ParticipantCommandObject participantCommandObject in roomCommandObject.currentParticipantCommandObjects)
                {
                    if (localParticipant != null && participantCommandObject.participantId.Equals(localParticipant.ParticipantId))
                    {
                        localParticipant.UpdateFromCommandObject(participantCommandObject);
                        continue;
                    }

                    SbRemoteParticipant remoteParticipant = _participantCollection.FindRemoteParticipant(participantCommandObject.participantId);
                    if (remoteParticipant == null)
                    {
                        remoteParticipant = _participantCollection.InsertRemoteParticipant(participantCommandObject);
                        InvokeRemoteParticipantEvent(RemoteParticipantEventType.Entered, remoteParticipant);
                    }
                    else
                    {
                        remoteParticipant.UpdateFromCommandObject(participantCommandObject);
                    }
                }
            }

            InvokeEventAndRemoveAllExitedRemoteParticipants();
        }

        private bool TryResetCustomItems(ReadOnlyDictionary<string, string> customItems, long effectedAt)
        {
            if (effectedAt < _customItemsUpdateAt)
                return false;

            if (customItems == null)
            {
                CustomItems = EMPTY_READ_ONLY_CUSTOM_ITEMS;
            }
            else
            {
                CustomItems = customItems;
            }

            _customItemsUpdateAt = effectedAt;
            return true;
        }

        private void OnRoomDeleted()
        {
            if (State == SbRoomState.Deleted)
            {
                Logger.LogInfo(Logger.CategoryType.Room, "Room::OnRoomDeleted room has already been deleted.");
                return;
            }

            Logger.LogInfo(Logger.CategoryType.Room, "Room::OnRoomDeleted");

            State = SbRoomState.Deleted;

            OnLocalParticipantExited(null);

            for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
            {
                _roomEventListeners[listenerIndex].OnDeleted();
            }

            _participantCollection.ClearAllParticipants();
        }

        private void OnLocalParticipantExited(SbError error)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"Room::OnLocalParticipantExited ErrorCode:{error?.ErrorCode} ErrorMessage:{error?.ErrorMessage}");

            if (error != null)
            {
                InvokeErrorEvent(new SbError(SbErrorCode.LocalParticipantLostConnection), LocalParticipant);
            }

            _participantManager?.OnLocalParticipantExited(LocalParticipant);
            _participantManager = null;

            _participantCollection.OnLocalParticipantExited();
            StopAliveCommand();
        }

        private bool OnRemoteParticipantExited(string participantId)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"Room::OnRemoteParticipantExited id:{participantId}");

            SbRemoteParticipant remoteParticipant = _participantCollection.FindRemoteParticipant(participantId);
            if (remoteParticipant != null)
            {
                _participantManager?.OnRemoteParticipantExited(remoteParticipant);
                InvokeRemoteParticipantEvent(RemoteParticipantEventType.Exited, remoteParticipant);
                _participantCollection.RemoveRemoteParticipant(participantId);
                return true;
            }

            return false;
        }

        private void InvokeRemoteParticipantEvent(RemoteParticipantEventType remoteParticipantEventType, SbRemoteParticipant remoteParticipant)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"Room::RemoteParticipantEvent EventType:{remoteParticipantEventType} ParticipantId:{remoteParticipant.ParticipantId}");

            if (IsEntered() == false)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::RemoteParticipantEvent Not entered yet.");
                return;
            }

            switch (remoteParticipantEventType)
            {
                case RemoteParticipantEventType.Entered:
                {
                    for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
                    {
                        _roomEventListeners[listenerIndex]?.OnRemoteParticipantEntered(remoteParticipant);
                    }

                    break;
                }
                case RemoteParticipantEventType.Exited:
                {
                    for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
                    {
                        _roomEventListeners[listenerIndex]?.OnRemoteParticipantExited(remoteParticipant);
                    }

                    break;
                }
                case RemoteParticipantEventType.AudioSettingsChanged:
                {
                    for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
                    {
                        _roomEventListeners[listenerIndex]?.OnRemoteAudioSettingsChanged(remoteParticipant);
                    }

                    break;
                }
                case RemoteParticipantEventType.VideoSettingsChanged:
                {
                    for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
                    {
                        _roomEventListeners[listenerIndex]?.OnRemoteAudioSettingsChanged(remoteParticipant);
                    }

                    break;
                }
            }
        }

        private void InvokeErrorEvent(SbError error, SbParticipantAbstract participant)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"Room::OnError ErrorCode:{error?.ErrorCode} ErrorMessage:{error?.ErrorMessage} ParticipantId:{participant?.ParticipantId}");

            for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
            {
                _roomEventListeners[listenerIndex].OnError(error, participant);
            }
        }

        internal void OnRoomCommandEvent(RoomEventCommandAbstract roomEventCommand)
        {
            if (roomEventCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnRoomCommandEvent Command is null");
                return;
            }

            if (roomEventCommand is ParticipantEnteredRoomEventCommand enteredEventCommand)
            {
                OnParticipantEnteredEventCommand(enteredEventCommand);
            }
            else if (roomEventCommand is ParticipantExitedRoomEventCommand exitedEventCommand)
            {
                OnParticipantExitedEventCommand(exitedEventCommand);
            }
            else if (roomEventCommand is ParticipantConnectedRoomEventCommand connectedEventCommand)
            {
                OnParticipantConnectedEventCommand(connectedEventCommand);
            }
            else if (roomEventCommand is ParticipantDisconnectedRoomEventCommand disconnectedEventCommand)
            {
                OnParticipantDisconnectedEventCommand(disconnectedEventCommand);
            }
            else if (roomEventCommand is ParticipantAudioStatusChangedRoomEventCommand audioStatusChangedEventCommand)
            {
                OnParticipantAudioStatusChangedEventCommand(audioStatusChangedEventCommand);
            }
            else if (roomEventCommand is ParticipantVideoStatusChangedRoomEventCommand videoStatusChangedEventCommand)
            {
                OnParticipantVideoStatusChangedEventCommand(videoStatusChangedEventCommand);
            }
            else if (roomEventCommand is RoomUpdateCustomItemsRoomEventCommand roomUpdateCustomItemsEventCommand)
            {
                OnRoomUpdateCustomItemsEventCommand(roomUpdateCustomItemsEventCommand);
            }
            else if (roomEventCommand is RoomDeleteCustomItemsRoomEventCommand roomDeleteCustomItemsEventCommand)
            {
                OnRoomDeleteCustomItemsEventCommand(roomDeleteCustomItemsEventCommand);
            }
            else if (roomEventCommand is RoomDeletedRoomEventCommand roomDeletedEventCommand)
            {
                OnRoomDeletedEventCommand(roomDeletedEventCommand);
            }
            else
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"Room::OnRoomCommandEvent Should implement event command type:{roomEventCommand.GetEventCommandType()}");
            }
        }

        private void OnParticipantEnteredEventCommand(ParticipantEnteredRoomEventCommand participantEnteredCommand)
        {
            if (participantEnteredCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnParticipantEnteredEventCommand Params is null or empty");
                return;
            }

            ParticipantCommandObject participantCommandObject = participantEnteredCommand.GetParticipantCommandObject();
            SbRemoteParticipant remoteParticipant = _participantCollection.FindRemoteParticipant(participantCommandObject.participantId);
            if (remoteParticipant == null)
            {
                remoteParticipant = _participantCollection.InsertRemoteParticipant(participantCommandObject);
                InvokeRemoteParticipantEvent(RemoteParticipantEventType.Entered, remoteParticipant);
            }
            else
            {
                remoteParticipant.UpdateFromCommandObject(participantCommandObject);
            }
        }

        private void OnParticipantExitedEventCommand(ParticipantExitedRoomEventCommand participantExitedCommand)
        {
            if (participantExitedCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnParticipantExitedEventCommand Params is null or empty");
                return;
            }

            ParticipantCommandObject participantCommandObject = participantExitedCommand.GetParticipantCommandObject();
            SbRemoteParticipant remoteParticipant = _participantCollection.FindRemoteParticipant(participantCommandObject.participantId);
            if (remoteParticipant != null)
            {
                InvokeRemoteParticipantEvent(RemoteParticipantEventType.Exited, remoteParticipant);
                _participantCollection.RemoveRemoteParticipant(remoteParticipant.ParticipantId);
            }
            else
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"Room::OnParticipantExitedEventCommand Invalid participant id:{participantCommandObject.participantId}");
            }
        }

        private void OnParticipantConnectedEventCommand(ParticipantConnectedRoomEventCommand participantConnectedCommand)
        {
            if (participantConnectedCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnParticipantConnectedEventCommand Params is null or empty");
                return;
            }

            ParticipantCommandObject participantCommandObject = participantConnectedCommand.GetParticipantCommandObject();
            SbRemoteParticipant remoteParticipant = _participantCollection.FindRemoteParticipant(participantCommandObject.participantId);
            if (remoteParticipant != null)
            {
                remoteParticipant.UpdateFromCommandObject(participantCommandObject);
                _participantManager?.OnRemoteParticipantConnected(remoteParticipant);
            }
            else
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"Room::OnParticipantConnectedEventCommand Invalid participant id:{participantCommandObject.participantId}");
            }
        }

        private void OnParticipantDisconnectedEventCommand(ParticipantDisconnectedRoomEventCommand participantDisconnectedCommand)
        {
            if (participantDisconnectedCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnParticipantDisconnectedEventCommand Params is null or empty");
                return;
            }

            ParticipantCommandObject participantCommandObject = participantDisconnectedCommand.GetParticipantCommandObject();
            if (LocalParticipant != null && LocalParticipant.ParticipantId.Equals(participantCommandObject.participantId))
            {
                OnLocalParticipantExited(new SbError(SbErrorCode.LocalParticipantLostConnection));
            }
            else if (OnRemoteParticipantExited(participantCommandObject.participantId) == false)
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"Room::OnParticipantDisconnectedEventCommand Invalid participant id:{participantCommandObject.participantId}");
            }
        }

        private void OnParticipantAudioStatusChangedEventCommand(ParticipantAudioStatusChangedRoomEventCommand participantAudioStatusChangedCommand)
        {
            if (participantAudioStatusChangedCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnParticipantAudioStatusChangedEventCommand Params is null or empty");
                return;
            }

            ParticipantCommandObject participantCommandObject = participantAudioStatusChangedCommand.GetParticipantCommandObject();
            SbRemoteParticipant remoteParticipant = _participantCollection.FindRemoteParticipant(participantCommandObject.participantId);
            if (remoteParticipant != null)
            {
                remoteParticipant.UpdateFromCommandObject(participantCommandObject);
                InvokeRemoteParticipantEvent(RemoteParticipantEventType.AudioSettingsChanged, remoteParticipant);
            }
            else
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"Room::OnParticipantAudioStatusChangedEventCommand Invalid participant id:{participantCommandObject.participantId}");
            }
        }

        private void OnParticipantVideoStatusChangedEventCommand(ParticipantVideoStatusChangedRoomEventCommand participantVideoStatusChangedCommand)
        {
            if (participantVideoStatusChangedCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnParticipantVideoStatusChangedEventCommand Params is null or empty");
                return;
            }

            ParticipantCommandObject participantCommandObject = participantVideoStatusChangedCommand.GetParticipantCommandObject();
            SbRemoteParticipant remoteParticipant = _participantCollection.FindRemoteParticipant(participantCommandObject.participantId);
            if (remoteParticipant != null)
            {
                remoteParticipant.UpdateFromCommandObject(participantCommandObject);
                InvokeRemoteParticipantEvent(RemoteParticipantEventType.VideoSettingsChanged, remoteParticipant);
            }
            else
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"Room::OnParticipantVideoStatusChangedEventCommand Invalid participant id:{participantCommandObject.participantId}");
            }
        }

        private void OnRoomUpdateCustomItemsEventCommand(RoomUpdateCustomItemsRoomEventCommand roomUpdateCustomItemsCommand)
        {
            if (roomUpdateCustomItemsCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnRoomUpdateCustomItemsEventCommand Params is null or empty");
                return;
            }

            bool isSucceeded = TryResetCustomItems(roomUpdateCustomItemsCommand.CustomItems, roomUpdateCustomItemsCommand.EffectedAt);
            if (isSucceeded)
            {
                for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
                {
                    _roomEventListeners[listenerIndex]?.OnCustomItemsUpdated(roomUpdateCustomItemsCommand.UpdatedKeys);
                }
            }
        }

        private void OnRoomDeleteCustomItemsEventCommand(RoomDeleteCustomItemsRoomEventCommand roomDeleteCustomItemsCommand)
        {
            if (roomDeleteCustomItemsCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnRoomDeleteCustomItemsEventCommand Params is null or empty");
                return;
            }

            bool isSucceeded = TryResetCustomItems(roomDeleteCustomItemsCommand.CustomItems, roomDeleteCustomItemsCommand.EffectedAt);
            if (isSucceeded)
            {
                for (int listenerIndex = _roomEventListeners.Count - 1; 0 <= listenerIndex; listenerIndex--)
                {
                    _roomEventListeners[listenerIndex]?.OnCustomItemsDeleted(roomDeleteCustomItemsCommand.DeletedKeys);
                }
            }
        }

        private void OnRoomDeletedEventCommand(RoomDeletedRoomEventCommand roomDeletedCommand)
        {
            if (roomDeletedCommand == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "Room::OnRoomDeletedEventCommand Params is null or empty");
                return;
            }

            OnRoomDeleted();
        }

        private void StartAliveCommand()
        {
            StopAliveCommand();
            _aliveCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(StartAliveCommandCoroutine());
        }

        private void StopAliveCommand()
        {
            if (_aliveCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_aliveCoroutine);
                _aliveCoroutine = null;
            }
        }

        private IEnumerator StartAliveCommandCoroutine()
        {
            Logger.LogInfo(Logger.CategoryType.Room, "StartAliveCommand");

            const int ALIVE_INTERVAL_SECONDS = 5;
            WaitForSecondsRealtime aliveIntervalYield = new WaitForSecondsRealtime(ALIVE_INTERVAL_SECONDS);
            while (true)
            {
                RoomAliveApiCommand.Request request = new RoomAliveApiCommand.Request(RoomId, LocalParticipant?.ParticipantId, AliveResultHandler);
                CommandRouter.Instance.Send(request);

                yield return aliveIntervalYield;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private void AliveResultHandler(ApiResponseAbstract apiResponse, SbError error)
        {
            if (error == null)
            {
                _aliveFailureCount = 0;
            }
            else
            {
                _aliveFailureCount += 1;

                Logger.LogWarning(Logger.CategoryType.Room, $"Failed room alive FailureCount:{_aliveFailureCount} ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage} ");

                const int ALIVE_RETRY_COUNT = 8;
                if (ALIVE_RETRY_COUNT <= _aliveFailureCount)
                {
                    OnLocalParticipantExited(new SbError(SbErrorCode.LocalParticipantLostConnection));
                }
                else if (error.ErrorCode == SbErrorCode.InvalidParticipantId)
                {
                    OnLocalParticipantExited(error);
                }
            }
        }

        private void InvokeEventAndRemoveAllExitedRemoteParticipants()
        {
            foreach (SbRemoteParticipant remoteParticipant in RemoteParticipants)
            {
                if (remoteParticipant.State == SbParticipantState.Exited)
                {
                    InvokeRemoteParticipantEvent(RemoteParticipantEventType.Exited, remoteParticipant);
                }
            }

            _participantCollection.RemoveAllExitedRemoteParticipants();
        }

        internal bool IsEntered()
        {
            return _participantCollection.HasLocalParticipant();
        }

        internal bool IsEnteringOrEntered()
        {
            return (IsEntered() || _isEntering);
        }

        internal void ForceExitLocalParticipant()
        {
            OnLocalParticipantExited(null);
        }
    }
}