// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    /// <summary>
    /// A enum that provides information about the state of a room.
    /// </summary>
    public enum SbRoomState
    {
        /// <summary>
        /// Indicates a room is open and available for group calls.
        /// </summary>
        [JsonProperty("open")] Open,
        /// <summary>
        /// Indicates a room is deleted.
        /// </summary>
        [JsonProperty("deleted")] Deleted
    }
}