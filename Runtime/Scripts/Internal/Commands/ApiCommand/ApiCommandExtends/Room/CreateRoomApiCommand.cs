// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class CreateRoomApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            internal struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("type")] internal string roomType;
                [JsonProperty("custom_items")] internal Dictionary<string, string> customItems;
#pragma warning restore CS0649
            }

            internal Request(SbRoomType roomType, Dictionary<string, string> customItems, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = "v1/rooms";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload { roomType = roomType.EnumToJsonPropertyName(), customItems = customItems };
                Payload = JsonConvert.SerializeObject(serializablePayload);
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("room")] internal readonly RoomCommandObject roomCommandObject = default;
        }
    }
}