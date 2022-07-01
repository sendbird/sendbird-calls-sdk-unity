// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    internal enum EventCommandCallType
    {
        [JsonProperty("")] None,
        [JsonProperty("room")] Room,
        [JsonProperty("direct_call")] DirectCall
    }
}