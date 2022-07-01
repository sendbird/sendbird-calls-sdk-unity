// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal struct UserCommandObject
    {
#pragma warning disable CS0649
        [JsonProperty("user_id")] internal readonly string userId;
        [JsonProperty("nickname")] internal readonly string nickname;
        [JsonProperty("profile_url")] internal readonly string profileURL;
        [JsonProperty("metadata")] internal readonly ReadOnlyDictionary<string, string> metadata;
        [JsonProperty("is_active")] internal readonly bool isActive;
#pragma warning restore CS0649
    }
}