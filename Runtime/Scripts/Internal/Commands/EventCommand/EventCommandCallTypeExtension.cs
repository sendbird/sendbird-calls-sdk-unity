// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal static class EventCommandCallTypeExtension
    {
        private static string ToJsonPropertyName(this EventCommandCallType eventCommandCallType)
        {
            return NewtonsoftJsonExtension.EnumToJsonPropertyName(eventCommandCallType);
        }

        internal static EventCommandCallType JsonPropertyNameToType(string jsonPropertyName)
        {
            if (EventCommandCallType.Room.ToJsonPropertyName().Equals(jsonPropertyName))
                return EventCommandCallType.Room;
            
            if (EventCommandCallType.DirectCall.ToJsonPropertyName().Equals(jsonPropertyName)) 
                return EventCommandCallType.DirectCall;

            return EventCommandCallType.None;
        }
    }
}