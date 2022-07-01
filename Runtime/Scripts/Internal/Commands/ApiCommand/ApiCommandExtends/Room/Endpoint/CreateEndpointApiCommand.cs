// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class CreateEndpointApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("sdp")] internal string sdp;
                [JsonProperty("audio_attr")] internal string AudioEndpointDirectionType;
                [JsonProperty("video_attr")] internal string VideoEndpointDirectionType;
#pragma warning restore CS0649
            }

            internal Request(string roomId, string localParticipantId, string inSdp, EndpointType audioEndpointType,
                             EndpointType videoEndpointType, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants/{localParticipantId}/endpoints";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload
                {
                    sdp = inSdp, 
                    AudioEndpointDirectionType = audioEndpointType.EnumToJsonPropertyName(), 
                    VideoEndpointDirectionType = videoEndpointType.EnumToJsonPropertyName(),
                };
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