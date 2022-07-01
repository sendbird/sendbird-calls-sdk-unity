// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    internal enum LogLevel
    {
        [JsonProperty("verbose")] Verbose,
        [JsonProperty("debug")] Debug,
        [JsonProperty("info")] Info,
        [JsonProperty("warning")] Warning,
        [JsonProperty("error")] Error,
        [JsonProperty("none")] None
    }
}