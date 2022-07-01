// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class GetRoomApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            internal Request(string roomId, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}";
                HttpMethod = UnityWebRequest.kHttpVerbGET;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("room")] internal readonly RoomCommandObject roomCommandObject = default;
        }
    }
}