// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal static class SbRoomStateExtension
    {
        internal static string EnumToJsonPropertyName(this SbRoomState roomState)
        {
            return NewtonsoftJsonExtension.EnumToJsonPropertyName(roomState);
        }

        internal static SbRoomState JsonPropertyNameToType(string jsonPropertyName)
        {
            if (SbRoomState.Open.EnumToJsonPropertyName().Equals(jsonPropertyName))
                return SbRoomState.Open;

            return SbRoomState.Deleted;
        }
    }
}