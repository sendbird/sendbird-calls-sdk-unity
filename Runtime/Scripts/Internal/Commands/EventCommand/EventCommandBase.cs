// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal class EventCommandBase
    {
        [JsonProperty("type")] internal readonly string CommandType = null;
        [JsonProperty("message_id")] internal readonly string MessageId = null;

        private EventCommandType _cachedEventCommandType = EventCommandType.None;

        internal EventCommandType GetEventCommandType()
        {
            if (_cachedEventCommandType == EventCommandType.None)
                _cachedEventCommandType = EventCommandTypeExtension.JsonPropertyNameToType(CommandType);

            return _cachedEventCommandType;
        }

        internal virtual bool CheckValid()
        {
            if (string.IsNullOrEmpty(CommandType))
            {
                Logger.LogWarning(Logger.CategoryType.Command, "EventCommand CommandType is null or empty");
                return false;
            }

            return true;
        }
    }
}