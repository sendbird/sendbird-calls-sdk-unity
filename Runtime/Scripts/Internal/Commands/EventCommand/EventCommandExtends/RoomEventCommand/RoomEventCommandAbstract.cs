// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal abstract class RoomEventCommandAbstract : EventCommandBase
    {
        [JsonProperty("call_type")] internal readonly string CallType = default;
        [JsonProperty("version")] internal readonly int Version = 0;

        internal abstract string RoomId { get; }

        internal override bool CheckValid()
        {
            if (base.CheckValid() == false)
                return false;

            if (EventCommandCallTypeExtension.JsonPropertyNameToType(CallType) != EventCommandCallType.Room)
            {
                Logger.LogWarning(Logger.CategoryType.Command, $"EventCommand CallType is invalid. type:{CommandType} call_type:{CallType}");
                return false;
            }

            return true;
        }
    }
}