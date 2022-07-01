// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.Generic;
using System.Linq;

namespace Sendbird.Calls
{
    internal class RoomManager : SingletonAbstract<RoomManager>
    {
        private readonly Dictionary<string, SbRoom> _roomsById = new Dictionary<string, SbRoom>();

        private RoomManager() { }

        internal SbRoomListQuery CreateRoomListQuery(SbRoomListQueryParams roomListQueryParams = null)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"CreateRoomListQuery Limit:{roomListQueryParams?.Limit} RoomStateType:{roomListQueryParams?.RoomState}");
            
            if (roomListQueryParams == null)
            {
                roomListQueryParams = new SbRoomListQueryParams();
            }
            return new SbRoomListQuery(roomListQueryParams);
        }

        internal void CreateRoom(SbRoomParams roomParams, SbRoomHandler roomHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"CreateRoom RoomType:{SbRoomType.LargeRoomForAudioOnly} CustomItemsCount:{roomParams?.CustomItems?.Count}");

            if (roomParams == null)
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("RoomParams");
                Logger.LogWarning(Logger.CategoryType.Common, $"RoomManager::CreateRoom {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => roomHandler?.Invoke(null, error));
                return;
            }
            
            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error == null)
                {
                    if (apiResponse is CreateRoomApiCommand.Response createRoomResponseAbstract)
                    {
                        SbRoom room = new SbRoom(createRoomResponseAbstract.roomCommandObject);
                        CacheRoom(room);
                        roomHandler?.Invoke(room, null);
                    }
                    else
                    {
                        roomHandler?.Invoke(null, new SbError(SbErrorCode.MalformedData));
                    }
                }
                else
                {
                    roomHandler?.Invoke(null, error);
                }
            }

            CreateRoomApiCommand.Request request = new CreateRoomApiCommand.Request(SbRoomType.LargeRoomForAudioOnly, roomParams.CustomItems, ResultHandler);
            CommandRouter.Instance.Send(request);
        }

        internal void CacheRoom(SbRoom room)
        {
            if (room == null || string.IsNullOrEmpty(room.RoomId))
            {
                Logger.LogWarning(Logger.CategoryType.Room, "CacheRoom room is null or invalid room id");
                return;
            }

            _roomsById[room.RoomId] = room;
        }

        internal SbRoom GetCachedRoomById(string roomId)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"GetCachedRoomById RoomId:{roomId}");
            if (_roomsById.TryGetValue(roomId, out SbRoom outRoom))
                return outRoom;

            return null;
        }

        internal void FetchRoomById(string roomId, SbRoomHandler roomHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"FetchRoomById RoomId:{roomId}");

            if (string.IsNullOrEmpty(roomId))
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("RoomId");
                Logger.LogWarning(Logger.CategoryType.Common, $"RoomManager::FetchRoomById {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => roomHandler?.Invoke(null, error));
                return;
            }
            
            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error == null)
                {
                    if (apiResponse is GetRoomApiCommand.Response getRoomResponseAbstract)
                    {
                        SbRoom room = new SbRoom(getRoomResponseAbstract.roomCommandObject);
                        CacheRoom(room);
                        roomHandler?.Invoke(room, null);
                    }
                    else
                    {
                        roomHandler?.Invoke(null, new SbError(SbErrorCode.MalformedData));
                    }
                }
                else
                {
                    roomHandler?.Invoke(null, error);
                }
            }

            GetRoomApiCommand.Request request = new GetRoomApiCommand.Request(roomId, ResultHandler);
            CommandRouter.Instance.Send(request);
        }

        internal void OnRoomCommandEvent(RoomEventCommandAbstract roomEventCommand)
        {
            if (roomEventCommand == null || string.IsNullOrEmpty(roomEventCommand.RoomId))
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"RoomManager::OnRoomCommandEvent Command is null or invalid room id:{roomEventCommand?.RoomId} command:{roomEventCommand?.CommandType}");
                return;
            }

            if (_roomsById.TryGetValue(roomEventCommand.RoomId, out SbRoom outRoom))
            {
                outRoom.OnRoomCommandEvent(roomEventCommand);
            }
            else
            {
                Logger.LogWarning(Logger.CategoryType.Room, $"RoomManager::OnRoomCommandEvent Invalid room id:{roomEventCommand.RoomId} command:{roomEventCommand.CommandType}");
            }
        }

        internal void OnApplicationQuit()
        {
            foreach (SbRoom sbRoom in _roomsById.Values)
            {
                if (sbRoom.IsEntered())
                {
                    sbRoom.ForceExitLocalParticipant();
                }
            }
            _roomsById.Clear();
        }

        internal bool HasEnteringOrEnteredRoom()
        {
            return _roomsById.Values.Any(room => room.IsEnteringOrEntered());
        }
    }
}