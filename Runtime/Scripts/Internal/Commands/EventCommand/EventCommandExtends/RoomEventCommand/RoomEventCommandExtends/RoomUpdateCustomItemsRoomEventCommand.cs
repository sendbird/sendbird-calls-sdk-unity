// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal class RoomUpdateCustomItemsRoomEventCommand : RoomEventCommandAbstract
    {
        [JsonProperty("payload")] private Payload _payload = default;

        internal override string RoomId => _payload.roomId;
        internal ReadOnlyDictionary<string, string> CustomItems => _payload.customItems;
        internal ReadOnlyCollection<string> UpdatedKeys => _payload.updatedKeys;
        internal long EffectedAt => _payload.affectedAt;

        internal override bool CheckValid()
        {
            if (base.CheckValid() == false)
                return false;

            if (string.IsNullOrEmpty(_payload.roomId))
            {
                Logger.LogWarning(Logger.CategoryType.Command, $"EventCommand RoomId is null or empty. type:{CommandType}");
                return false;
            }

            return true;
        }

        [Serializable]
        internal struct Payload
        {
#pragma warning disable CS0649
            [JsonProperty("room_id")] internal readonly string roomId;
            [JsonProperty("custom_items")] internal readonly ReadOnlyDictionary<string, string> customItems;
            [JsonProperty("updated")] internal readonly ReadOnlyCollection<string> updatedKeys;
            [JsonProperty("affected_at")] internal readonly long affectedAt;
#pragma warning restore CS0649
        }
    }
}