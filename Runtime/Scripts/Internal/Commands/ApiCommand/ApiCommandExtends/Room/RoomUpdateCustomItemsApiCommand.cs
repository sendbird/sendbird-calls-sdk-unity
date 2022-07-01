// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class RoomUpdateCustomItemsApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("custom_items")] internal Dictionary<string, string> customItems;
                [JsonProperty("mode")] internal string updateMode;
#pragma warning restore CS0649
            }

            internal Request(string roomId, Dictionary<string, string> customItems, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/custom_items";
                HttpMethod = UnityWebRequest.kHttpVerbPUT;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                const string UPDATE_MODE = "upsert";
                SerializablePayload serializablePayload = new SerializablePayload { customItems = customItems, updateMode = UPDATE_MODE };
                Payload = JsonConvert.SerializeObject(serializablePayload);
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("affected_at")] internal readonly long affectedAt = 0;
            [JsonProperty("custom_items")] internal readonly ReadOnlyDictionary<string, string> customItems = null;
            [JsonProperty("updated")] internal readonly ReadOnlyCollection<string> updatedKeys = null;
        }
    }
}