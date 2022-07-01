// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal struct ParticipantCommandObject
    {
#pragma warning disable CS0649
        [JsonProperty("participant_id")] internal readonly string participantId;
        [JsonProperty("entered_at")] internal readonly long enteredAt;
        [JsonProperty("updated_at")] internal readonly long updatedAt;
        [JsonProperty("exit_at")] internal readonly long exitAt;
        [JsonProperty("duration")] internal readonly long duration;
        [JsonProperty("client_id")] internal readonly string clientId;
        [JsonProperty("state")] internal readonly string state;
        [JsonProperty("user")] internal readonly UserCommandObject userCommandObject;
        [JsonProperty("is_audio_on")] internal readonly bool isAudioOn;
        [JsonProperty("is_video_on")] internal readonly bool isVideoOn;
#pragma warning restore CS0649
    }
}