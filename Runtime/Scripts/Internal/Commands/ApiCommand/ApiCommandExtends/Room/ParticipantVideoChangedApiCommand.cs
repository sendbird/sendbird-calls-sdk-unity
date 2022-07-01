// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class ParticipantVideoChangedApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
                [JsonProperty("on")] internal bool isVideoOn;
            }

            internal Request(string roomId, string localParticipantId, bool videoOn, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants/{localParticipantId}/video";
                HttpMethod = UnityWebRequest.kHttpVerbPUT;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload { isVideoOn = videoOn };
                Payload = JsonConvert.SerializeObject(serializablePayload);
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("me")] internal readonly ParticipantCommandObject meParticipantCommandObject = default;
        }
    }
}