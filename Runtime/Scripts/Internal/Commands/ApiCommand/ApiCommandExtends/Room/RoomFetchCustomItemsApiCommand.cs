// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class RoomFetchCustomItemsApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            internal Request(string roomId, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/custom_items";
                HttpMethod = UnityWebRequest.kHttpVerbGET;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("affected_at")] internal readonly long affectedAt = 0;
            [JsonProperty("custom_items")] internal readonly ReadOnlyDictionary<string, string> customItems = null;
        }
    }
}