// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.Generic;

namespace Sendbird.Calls
{
    /// <summary>
    /// A class that provides information for creating a new room.
    /// </summary>
    public class SbRoomParams
    {
        /// <summary>
        /// Sets custom items for the room.
        /// </summary>
        public Dictionary<string, string> CustomItems { get; set; } = new Dictionary<string, string>();
    }
}