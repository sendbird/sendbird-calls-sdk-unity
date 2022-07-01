// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    internal enum EndpointType
    {
        [JsonProperty("sendrecv")] SendReceive,
        [JsonProperty("sendonly")] SendOnly,
        [JsonProperty("recvonly")] ReceiveOnly,
        [JsonProperty("none")] None
    }
}