// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    internal enum EventCommandType
    {
        [JsonProperty("login")] Login,
        [JsonProperty("log")] Log,

        //Room
        [JsonProperty("participant_entered")] ParticipantEntered,
        [JsonProperty("participant_connected")] ParticipantConnected,
        [JsonProperty("participant_exited")] ParticipantExited,
        [JsonProperty("participant_audio_status_changed")] ParticipantAudioStatusChanged,
        [JsonProperty("participant_video_status_changed")] ParticipantVideoStatusChanged,
        [JsonProperty("participant_disconnected")] ParticipantDisconnected,
        [JsonProperty("custom_items_update")] CustomItemsUpdate,
        [JsonProperty("custom_items_delete")] CustomItemsDelete,
        [JsonProperty("room_deleted")] RoomDeleted,
        [JsonProperty("")] None,
        Start = Login,
        Max = None
    }
}