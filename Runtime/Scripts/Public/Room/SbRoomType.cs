// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    /// <summary>
    /// An enum that represents different types of a room.
    /// </summary>
    public enum SbRoomType
    {
        /// <summary>
        /// Type of a room that only supports audio and can have up to 100 participants.
        /// </summary>
        [JsonProperty("large_room_for_audio_only")] LargeRoomForAudioOnly,
        /// <summary>
        /// Not supported yet.
        /// </summary>
        [JsonProperty("small_room_for_video")] SmallRoomForVideo
    }
}