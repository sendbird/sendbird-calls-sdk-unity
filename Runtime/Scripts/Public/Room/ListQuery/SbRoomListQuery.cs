// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// A class that is used to query Rooms.
    /// </summary>
    public partial class SbRoomListQuery
    {
        /// <summary>
        /// Indicates whether there are more rooms to be queried.
        /// </summary>
        public bool HasNext { get; private set; }

        /// <summary>
        /// Retrieves the list of rooms after the last result set.
        /// </summary>
        /// <param name="roomListQueryHandler">A callback handler which the result will be passed.</param>
        public void Next(SbRoomListQueryHandler roomListQueryHandler)
        {
            if (HasNext == false)
            {
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => roomListQueryHandler?.Invoke(EMPTY_READ_ONLY_ROOMS, null));
                return;
            }

            if (_isQuerying)
            {
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => roomListQueryHandler?.Invoke(EMPTY_READ_ONLY_ROOMS, new SbError(SbErrorCode.QueryInProgress)));
                return;
            }

            _roomListQueryHandler = roomListQueryHandler;
            _isQuerying = true;
            RoomListApiCommand.Request request = new RoomListApiCommand.Request(_sbRoomListQueryParams, _nextToken, RoomListResultHandler);
            CommandRouter.Instance.Send(request);
        }
    }
}