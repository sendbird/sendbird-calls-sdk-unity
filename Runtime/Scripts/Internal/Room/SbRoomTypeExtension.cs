// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal static class SbRoomTypeExtension
    {
        internal static string EnumToJsonPropertyName(this SbRoomType roomType)
        {
            return NewtonsoftJsonExtension.EnumToJsonPropertyName(roomType);
        }

        internal static SbRoomType JsonPropertyNameToType(string jsonPropertyName)
        {
            if (SbRoomType.LargeRoomForAudioOnly.EnumToJsonPropertyName().Equals(jsonPropertyName)) 
                return SbRoomType.LargeRoomForAudioOnly;

            return SbRoomType.SmallRoomForVideo;
        }
    }
}