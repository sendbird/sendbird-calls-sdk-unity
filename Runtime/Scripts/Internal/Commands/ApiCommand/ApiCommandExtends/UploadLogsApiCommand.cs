// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class UploadLogsApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            [Serializable]
            private struct SerializablePayload
            {
#pragma warning disable CS0649
                [JsonProperty("version")] internal int version;
                [JsonProperty("is_overflow_logs")] internal bool isOverflowLogs;
                [JsonProperty("logs")] internal LogItemCommandObject[] logCommandObjects;
#pragma warning restore CS0649
            }

            internal Request(LogItemCommandObject[] logItems, bool isOverflow)
            {
                URL = $"v1/sdk/debug_logs";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                IsSessionTokenRequired = true;

                SerializablePayload tempSerializablePayload = new SerializablePayload
                {
                    version = 1,
                    logCommandObjects = logItems, 
                    isOverflowLogs = isOverflow
                };
                Payload = JsonConvert.SerializeObject(tempSerializablePayload);
            }
        }
    }
}