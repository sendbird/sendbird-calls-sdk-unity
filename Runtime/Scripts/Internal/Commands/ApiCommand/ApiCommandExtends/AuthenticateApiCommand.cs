// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class AuthenticateApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("app_id")] internal string appId;
                [JsonProperty("access_token")] internal string accessToken;
#pragma warning restore CS0649
            }

            internal Request(string appId, string userId, string accessToken, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/users/{userId}/login";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                IsSessionTokenRequired = false;
                ResponseType = typeof(Response);
                ResultDelegate = resultDelegate;

                SerializablePayload tempSerializablePayload = new SerializablePayload { appId = appId, accessToken = accessToken };
                Payload = JsonConvert.SerializeObject(tempSerializablePayload);
            }
        }

        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            [JsonProperty("sbcall_session_token")] internal readonly string sessionToken = null;
            [JsonProperty("stats_interval")] internal readonly int statsInterval = 0;
            [JsonProperty("user")] internal readonly UserCommandObject userCommandObject = default;
        }
    }
}