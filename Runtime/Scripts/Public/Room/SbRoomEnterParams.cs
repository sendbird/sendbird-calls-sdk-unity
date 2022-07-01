// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// A class that provides the methods to enable audio settings.
    /// </summary>
    public class SbRoomEnterParams
    {
        /// <summary>
        /// Enables a participant's audio settings when entering a room.
        /// </summary>
        public bool AudioEnabled { get; set; } = true;
    }
}