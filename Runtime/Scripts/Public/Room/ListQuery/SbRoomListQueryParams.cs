// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.Generic;

namespace Sendbird.Calls
{
    /// <summary>
    /// A class that provides configuration for RoomListQuery.
    /// </summary>
    public partial class SbRoomListQueryParams
    {
        /// <summary>
        /// Filters query results to include rooms that were created between the specified range of time.
        /// </summary>
        public SbRange CreatedAtRange { get; set; } = null;
        /// <summary>
        /// Filters query results to include rooms that were created by specified user Ids.
        /// </summary>
        public List<string> CreatedByUserIds { get; set; } = new List<string>();
        /// <summary>
        /// Filters query results to include rooms with the specified range of numbers for current participants.
        /// </summary>
        public SbRange CurrentParticipantCountRange { get; set; } = null;
        /// <summary>
        /// The number of rooms to be retrieved at once.
        /// </summary>
        public int Limit { get; set; } = LIMIT_DEFAULT;
        /// <summary>
        /// Filters query results to include rooms that match the specified room Ids.
        /// </summary>
        public List<string> RoomIds { get; set; } = new List<string>();
        /// <summary>
        /// Filters query results to include room with the specified room state.
        /// </summary>
        public SbRoomState? RoomState { get; set; } = null;
    }
}