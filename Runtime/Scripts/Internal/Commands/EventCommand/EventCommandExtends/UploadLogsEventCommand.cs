// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal class UploadLogsEventCommand : EventCommandBase
    {
        [JsonProperty("log_level")] internal readonly string logLevel = default;
    }
}