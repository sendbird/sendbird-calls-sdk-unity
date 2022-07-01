// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal class CommandRouter : SingletonAbstract<CommandRouter>
    {
        private readonly ApiClient _apiClient = new ApiClient();
        private readonly WebSocketClient _webSocketClient = new WebSocketClient();
        private string _sessionToken = null;

        private CommandRouter() { }

        public void Init(string appId)
        {
            _apiClient.Init(appId);
            _webSocketClient.Init(ReceivedCommandHandler);
        }

        public void Terminate()
        {
            _apiClient.Terminate();
            _webSocketClient.Disconnect();
        }

        public void Send(ApiRequestAbstract apiRequest)
        {
            _apiClient.Send(apiRequest);
        }

        public void ConnectWebSocketAsync()
        {
            _webSocketClient.ConnectAsync();
        }

        public void SetSessionToken(string sessionToken)
        {
            _sessionToken = sessionToken;
            _apiClient.SetSessionToken(_sessionToken);
            _webSocketClient.SetSessionToken(_sessionToken);
        }

        private void ReceivedCommandHandler(string json)
        {
            EventCommandBase eventCommand = EventCommandFactory.CreateEventCommandFromJson(json);
            if (eventCommand == null || eventCommand.CheckValid() == false)
                return;

            if (eventCommand is RoomEventCommandAbstract roomEventCommand)
            {
                RoomManager.Instance.OnRoomCommandEvent(roomEventCommand);
            }
            else if (eventCommand is UploadLogsEventCommand uploadLogsEventCommand)
            {
                Logger.SendLogsToServer(LogLevelExtension.JsonPropertyNameToType(uploadLogsEventCommand.logLevel));
            }
            else
            {
                Logger.LogInfo(Logger.CategoryType.Command, $"Command is not room event. type:{eventCommand.GetEventCommandType()}");
            }
        }
    }
}