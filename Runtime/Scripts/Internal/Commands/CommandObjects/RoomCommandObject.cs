// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal struct RoomCommandObject
    {
#pragma warning disable CS0649
        [JsonProperty("room_id")] internal readonly string roomId;
        [JsonProperty("created_at")] internal readonly long createdAt;
        [JsonProperty("updated_at")] internal readonly long updatedAt;
        [JsonProperty("created_by")] internal readonly string createdBy;
        [JsonProperty("deleted_by")] internal readonly string deletedBy;
        [JsonProperty("state")] internal readonly string state;
        [JsonProperty("room_type")] internal readonly string roomType;
        [JsonProperty("current_participants")] internal readonly ReadOnlyCollection<ParticipantCommandObject> currentParticipantCommandObjects;
        [JsonProperty("custom_items")] internal readonly ReadOnlyDictionary<string, string> customItems;
#pragma warning restore CS0649
    }
}