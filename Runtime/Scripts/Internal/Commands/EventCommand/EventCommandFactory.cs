// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Newtonsoft.Json;

namespace Sendbird.Calls
{
    internal static class EventCommandFactory
    {
        internal static EventCommandBase CreateEventCommandFromJson(string json)
        {
            EventCommandBase eventCommandBase = JsonConvert.DeserializeObject<EventCommandBase>(json);
            if (eventCommandBase == null)
            {
                Logger.LogWarning(Logger.CategoryType.Command, $"EventCommandFactory::CreateEventCommandFromJson Command is null. json:{json}");
                return null;
            }

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            switch (eventCommandBase.GetEventCommandType())
            {
                case EventCommandType.ParticipantEntered:            return JsonConvert.DeserializeObject<ParticipantEnteredRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.ParticipantExited:             return JsonConvert.DeserializeObject<ParticipantExitedRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.ParticipantConnected:          return JsonConvert.DeserializeObject<ParticipantConnectedRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.ParticipantDisconnected:       return JsonConvert.DeserializeObject<ParticipantDisconnectedRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.ParticipantAudioStatusChanged: return JsonConvert.DeserializeObject<ParticipantAudioStatusChangedRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.ParticipantVideoStatusChanged: return JsonConvert.DeserializeObject<ParticipantExitedRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.CustomItemsUpdate:             return JsonConvert.DeserializeObject<RoomUpdateCustomItemsRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.CustomItemsDelete:             return JsonConvert.DeserializeObject<RoomDeleteCustomItemsRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.RoomDeleted:                   return JsonConvert.DeserializeObject<RoomDeletedRoomEventCommand>(json, jsonSerializerSettings);
                case EventCommandType.Login:                         return JsonConvert.DeserializeObject<EventCommandBase>(json, jsonSerializerSettings);
                case EventCommandType.Log:                           return JsonConvert.DeserializeObject<UploadLogsEventCommand>(json, jsonSerializerSettings);
            }

            Logger.LogWarning(Logger.CategoryType.Command, $"EventCommandFactory::CreateEventCommandFromJson Invalid event command. type:{eventCommandBase.CommandType}");
            return null;
        }
    }
}