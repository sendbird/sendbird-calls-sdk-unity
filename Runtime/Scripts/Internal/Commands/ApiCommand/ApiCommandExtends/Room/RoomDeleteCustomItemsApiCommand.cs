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
    internal sealed class RoomDeleteCustomItemsApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("keys")] internal List<string> keys;
#pragma warning restore CS0649
            }

            internal Request(string roomId, List<string> customItemKeys, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/custom_items";
                HttpMethod = UnityWebRequest.kHttpVerbDELETE;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload { keys = customItemKeys };
                Payload = JsonConvert.SerializeObject(serializablePayload);
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("affected_at")] internal readonly long affectedAt = 0;
            [JsonProperty("custom_items")] internal readonly ReadOnlyDictionary<string, string> customItems = null;
            [JsonProperty("deleted")] internal readonly ReadOnlyCollection<string> deletedKeys = null;
        }
    }
}