// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class RoomAttachApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("attach_to")] internal string attachTo;
#pragma warning restore CS0649
            }

            internal Request(string roomId, string localParticipantId, string endpointId, string attachToParticipantId, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants/{localParticipantId}/endpoints/{endpointId}/attach";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload { attachTo = attachToParticipantId };
                Payload = JsonConvert.SerializeObject(serializablePayload);
            }
        }
    }
}