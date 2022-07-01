// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal class RoomDeletedRoomEventCommand : RoomEventCommandAbstract
    {
        [JsonProperty("payload")] private Payload _payload = default;

        internal override string RoomId => _payload.roomId;
        internal string DeletedBy => _payload.deletedBy;
        internal long DeletedAt => _payload.deletedAt;

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
            [JsonProperty("deleted_at")] internal readonly long deletedAt;
            [JsonProperty("deleted_by")] internal readonly string deletedBy;
#pragma warning restore CS0649
        }
    }
}