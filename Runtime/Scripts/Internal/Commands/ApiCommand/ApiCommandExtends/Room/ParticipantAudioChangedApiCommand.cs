// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class ParticipantAudioChangedApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
                [JsonProperty("on")] internal bool isAudioOn;
            }

            internal Request(string roomId, string localParticipantId, bool audioOn, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants/{localParticipantId}/audio";
                HttpMethod = UnityWebRequest.kHttpVerbPUT;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload { isAudioOn = audioOn };
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