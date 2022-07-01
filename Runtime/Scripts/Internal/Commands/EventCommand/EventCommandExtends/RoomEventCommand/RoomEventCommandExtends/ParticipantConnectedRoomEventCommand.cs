// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;

namespace Sendbird.Calls
{
    [Serializable]
    internal class ParticipantConnectedRoomEventCommand : RoomEventCommandAbstract
    {
        [JsonProperty("payload")] private Payload _payload = default;

        internal override string RoomId => _payload.roomId;

        internal ParticipantCommandObject GetParticipantCommandObject()
        {
            return _payload.participantCommandObject;
        }

        internal override bool CheckValid()
        {
            if (base.CheckValid() == false)
                return false;

            if (string.IsNullOrEmpty(_payload.roomId))
            {
                Logger.LogWarning(Logger.CategoryType.Command, $"EventCommand RoomId is null or empty. type:{CommandType}");
                return false;
            }

            if (string.IsNullOrEmpty(_payload.participantCommandObject.participantId))
            {
                Logger.LogWarning(Logger.CategoryType.Command, $"EventCommand ParticipantId is null or empty. type:{CommandType}");
                return false;
            }

            return true;
        }

        [Serializable]
        internal struct Payload
        {
#pragma warning disable CS0649
            [JsonProperty("room_id")] internal readonly string roomId;
            [JsonProperty("participant")] internal readonly ParticipantCommandObject participantCommandObject;
#pragma warning restore CS0649
        }
    }
}