// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class EnterRoomApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
                [JsonProperty("is_audio_on")] internal bool isAudioOn;
                [JsonProperty("is_video_on")] internal bool isVideoOn;
            }

            internal Request(string roomId, bool audioOn, bool videoOn, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload serializablePayload = new SerializablePayload { isAudioOn = audioOn, isVideoOn = videoOn };
                Payload = JsonConvert.SerializeObject(serializablePayload);
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("me")] internal readonly ParticipantCommandObject meParticipantCommandObject = default;
            [JsonProperty("room")] internal readonly RoomCommandObject roomCommandObject = default;
        }
    }
}