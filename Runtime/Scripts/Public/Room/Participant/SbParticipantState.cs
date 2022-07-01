// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    /// <summary>
    ///  A class that provides information about the state of a participant.
    /// </summary>
    public enum SbParticipantState
    {
        /// <summary>
        /// Indicates that a participant entered the room.
        /// </summary>
        [JsonProperty("entered")] Entered,
        /// <summary>
        /// Indicates that a participant is connected and streaming media.
        /// </summary>
        [JsonProperty("connected")] Connected,
        /// <summary>
        /// Indicates that a participant exited the room.
        /// </summary>
        [JsonProperty("exited")] Exited
    }
}