// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sendbird.Calls
{
    public partial class SbRoomListQuery
    {
        private static readonly ReadOnlyCollection<SbRoom> EMPTY_READ_ONLY_ROOMS = new ReadOnlyCollection<SbRoom>(new List<SbRoom>(0));

        private readonly SbRoomListQueryParams _sbRoomListQueryParams;
        private bool _isQuerying = false;
        private string _nextToken = null;
        private SbRoomListQueryHandler _roomListQueryHandler;

        internal SbRoomListQuery(SbRoomListQueryParams roomListQueryParams)
        {
            _sbRoomListQueryParams = roomListQueryParams;
            _isQuerying = false;
            HasNext = true;
            _nextToken = null;
        }

        private void RoomListResultHandler(ApiResponseAbstract apiResponseAbstract, SbError error)
        {
            _isQuerying = false;
            HasNext = false;
            if (error != null)
            {
                _roomListQueryHandler?.Invoke(EMPTY_READ_ONLY_ROOMS, error);
                _roomListQueryHandler = null;
                return;
            }

            RoomListApiCommand.Response roomListResponse = apiResponseAbstract as RoomListApiCommand.Response;
            if (roomListResponse == null)
            {
                _roomListQueryHandler?.Invoke(EMPTY_READ_ONLY_ROOMS, new SbError(SbErrorCode.MalformedData));
                _roomListQueryHandler = null;
                return;
            }

            _nextToken = roomListResponse.Next;
            if (string.IsNullOrEmpty(_nextToken) == false) HasNext = true;

            if (0 < roomListResponse.roomCommandObjects.Count)
            {
                List<SbRoom> sbRooms = new List<SbRoom>(roomListResponse.roomCommandObjects.Count);
                foreach (RoomCommandObject roomCommandObject in roomListResponse.roomCommandObjects)
                {
                    if (SbRoomTypeExtension.JsonPropertyNameToType(roomCommandObject.roomType) != SbRoomType.LargeRoomForAudioOnly)
                    {
                        continue;
                    }

                    SbRoom sbRoom = new SbRoom(roomCommandObject);
                    sbRooms.Add(sbRoom);
                    RoomManager.Instance.CacheRoom(sbRoom);
                }

                _roomListQueryHandler?.Invoke(new ReadOnlyCollection<SbRoom>(sbRooms), null);
            }
            else
            {
                _roomListQueryHandler?.Invoke(EMPTY_READ_ONLY_ROOMS, null);
            }
            
            _roomListQueryHandler = null;
        }
    }
}