// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    internal struct LogItemCommandObject
    {
#pragma warning disable CS0649
        [JsonProperty("log_dt")] internal readonly long time;
        [JsonProperty("log_level")] internal readonly LogLevel logLevel;
        [JsonProperty("log_message")] internal readonly string message;
#pragma warning restore CS0649
        
        internal LogItemCommandObject(long time, LogLevel logLevel, string message)
        {
            this.time = time;
            this.logLevel = logLevel;
            this.message = message;
        }
    }
}