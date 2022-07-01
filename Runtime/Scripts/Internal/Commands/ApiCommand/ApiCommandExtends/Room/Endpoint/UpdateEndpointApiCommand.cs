// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class UpdateEndpointApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("sdp")] internal string sdp;
#pragma warning restore CS0649
            }

            internal Request(string roomId, string localParticipantId, string endpointId, string sdp, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants/{localParticipantId}/endpoints/{endpointId}";
                HttpMethod = UnityWebRequest.kHttpVerbPUT;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload { sdp = sdp };
                Payload = JsonConvert.SerializeObject(serializablePayload);
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("endpoint_id")] internal readonly string endpointId = null;
            [JsonProperty("sdp")] internal readonly string sdp = null;
        }
    }
}