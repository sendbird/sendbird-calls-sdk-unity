// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal static class EventCommandTypeExtension
    {
        private static string ToJsonPropertyName(this EventCommandType eventCommandType)
        {
            return NewtonsoftJsonExtension.EnumToJsonPropertyName(eventCommandType);
        }

        internal static EventCommandType JsonPropertyNameToType(string jsonPropertyName)
        {
            for (EventCommandType commandType = EventCommandType.Start; commandType < EventCommandType.Max; commandType++)
                if (commandType.ToJsonPropertyName().Equals(jsonPropertyName))
                    return commandType;

            return EventCommandType.None;
        }
    }
}