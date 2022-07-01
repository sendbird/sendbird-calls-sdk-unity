// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Sendbird.Calls
{
    internal class WebSocketClient
    {
        internal delegate void ReceiveEventCommandDelegate(string json);

        private ClientWebSocket _webSocket;
        private string _sessionToken = null;
        private ReceiveEventCommandDelegate _receiveEventCommandDelegate = null;

        private readonly BlockingCollection<string> _receiveQueue = new BlockingCollection<string>();
        private Coroutine _flushCoroutine = null;
        private Coroutine _reconnectingCoroutine = null;
        private Coroutine _checkAliveCoroutine = null;
        private WebSocketClientStateType _stateType = WebSocketClientStateType.None;

        internal void Init(ReceiveEventCommandDelegate receiveEventCommandDelegate)
        {
            _receiveEventCommandDelegate = receiveEventCommandDelegate;
        }

        internal async void ConnectAsync()
        {
            string appId = SendbirdCallSdk.Instance.AppId;
            string clientId = SendbirdCallSdk.Instance.ClientId;

            Logger.LogInfo(Logger.CategoryType.WebSocket, $"Connect start applicationId:{appId} sessionToken:{_sessionToken} clientId:{clientId}");
            if (_stateType == WebSocketClientStateType.Connecting || _stateType == WebSocketClientStateType.Open)
            {
                Logger.LogInfo(Logger.CategoryType.WebSocket, $"Already connected State:{_stateType}");
                return;
            }

            if (_flushCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_flushCoroutine);
                _flushCoroutine = null;
            }

            _webSocket = new ClientWebSocket();

            ChangeWebsocketState(WebSocketClientStateType.Connecting);

            StringBuilder uriStringBuilder = new StringBuilder($"wss://ws-{appId}.calls.sendbird.com");
            uriStringBuilder.Append($"/?sendbird_app_id={appId}");
            uriStringBuilder.Append($"&sbcall_session_token={_sessionToken}");
            uriStringBuilder.Append($"&sbcall_client_id={clientId}");

            _webSocket.Options.SetRequestHeader("User-Agent", ApiClient.HEADER_USER_AGENT);
            _webSocket.Options.SetRequestHeader("Request-Sent-Timestamp", DateTime.Now.Millisecond.ToString());

            try
            {
                Uri uri = new Uri(uriStringBuilder.ToString());
                Logger.LogInfo(Logger.CategoryType.WebSocket, $"connect to URI:{uri}");
                await _webSocket.ConnectAsync(uri, CancellationToken.None);
            }
            catch (Exception exception)
            {
                Logger.LogWarning(Logger.CategoryType.WebSocket, $"WebSocketClient::ConnectAsync Exception:{exception.Message}");
                CloseAndReconnect();
                return;
            }

            ChangeWebsocketState(WebSocketClientStateType.Open);

            _flushCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(FlushCoroutine());
            _checkAliveCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(CheckAliveCoroutine());
            WaitForReceiveAsync();
        }

        internal void Disconnect()
        {
            ChangeWebsocketState(WebSocketClientStateType.Disconnected);

            CloseAsync();
        }

        private async void CloseAsync()
        {
            if (_flushCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_flushCoroutine);
                _flushCoroutine = null;
            }
            
            if (_checkAliveCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_checkAliveCoroutine);
                _checkAliveCoroutine = null;
            }
            
            if (_webSocket != null && (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.Connecting))
            {
                Logger.LogInfo(Logger.CategoryType.WebSocket, $"CloseAsync start");
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                catch (Exception exception)
                {
                    Logger.LogWarning(Logger.CategoryType.WebSocket, $"WebSocketClient::CloseAsync Exception:{exception.Message}");
                }

                _webSocket.Dispose();
                _webSocket = null;
                Logger.LogInfo(Logger.CategoryType.WebSocket, $"CloseAsync end");
            }
        }

        private void CloseAndReconnect()
        {
            if (_reconnectingCoroutine != null)
            {
                SendbirdCallGameObject.Instance.StopCoroutine(_reconnectingCoroutine);
                _reconnectingCoroutine = null;
            }

            if (_stateType != WebSocketClientStateType.Disconnected)
            {
                _reconnectingCoroutine = SendbirdCallGameObject.Instance.StartCoroutine(ReconnectCoroutine());    
            }
        }

        private IEnumerator ReconnectCoroutine()
        {
            ChangeWebsocketState(WebSocketClientStateType.Closing);
            
            CloseAsync();

            while (_webSocket != null)
            {
                yield return null;
            }
            
            ChangeWebsocketState(WebSocketClientStateType.Closed);

            yield return new WaitForSecondsRealtime(1f);

            ConnectAsync();
            _reconnectingCoroutine = null;
        }
        
        private IEnumerator CheckAliveCoroutine()
        {
            WaitForSecondsRealtime waitForCheckInterval = new WaitForSecondsRealtime(0.3f);
            while (RoomManager.Instance.HasEnteringOrEnteredRoom())
            {
                yield return waitForCheckInterval;
            }
            
            Disconnect();
            _checkAliveCoroutine = null;
        }

        private IEnumerator FlushCoroutine()
        {
            while (_stateType == WebSocketClientStateType.Open || 0 < _receiveQueue.Count)
            {
                if (0 < _receiveQueue.Count && _receiveEventCommandDelegate != null)
                {
                    string json = _receiveQueue.Take();
                    Logger.LogInfo(Logger.CategoryType.WebSocket, $"Flush received queue\n {json}");
                    _receiveEventCommandDelegate.Invoke(json);
                }

                yield return null;
            }
        }

        private void ChangeWebsocketState(WebSocketClientStateType webSocketStateType)
        {
            if (_stateType != webSocketStateType)
            {
                Logger.LogInfo(Logger.CategoryType.WebSocket, $"ChangeWebsocketState {_stateType}->{webSocketStateType}");
                _stateType = webSocketStateType;
            }
        }

        private async void WaitForReceiveAsync()
        {
            Logger.LogInfo(Logger.CategoryType.WebSocket, "WaitForReceiveAsync started...");

            ArraySegment<byte> buffer = WebSocket.CreateClientBuffer(1024, 1024);
            if (buffer.Array == null)
            {
                Logger.LogError(Logger.CategoryType.WebSocket, "WaitForReceiveAsync buffer.Array is null");
                return;
            }

            while (_webSocket?.State == WebSocketState.Open)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    WebSocketReceiveResult webSocketReceiveResult = null;
                    do
                    {
                        try
                        {
                            webSocketReceiveResult = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);
                            memoryStream.Write(buffer.Array, buffer.Offset, webSocketReceiveResult.Count);
                        }
                        catch (Exception exception)
                        {
                            Logger.LogWarning(Logger.CategoryType.WebSocket, $"WebSocketClient::WaitForReceiveAsync Exception:{exception.Message}");
                            CloseAndReconnect();
                            return;
                        }
                    } while (!webSocketReceiveResult.EndOfMessage);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Text)
                    {
                        using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8))
                        {
                            Task<string> readToEndTask = streamReader.ReadToEndAsync();
                            await readToEndTask;

                            Logger.LogInfo(Logger.CategoryType.WebSocket, $"Receive {readToEndTask.Result}");
                            _receiveQueue.Add(readToEndTask.Result);
                        }
                    }
                    else if (webSocketReceiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        Logger.LogInfo(Logger.CategoryType.WebSocket, $"Receive result is close CloseStatus:{webSocketReceiveResult.CloseStatus?.ToString()} CloseStatusDescription:{webSocketReceiveResult.CloseStatusDescription}");
                        CloseAndReconnect();
                    }
                    else
                    {
                        Logger.LogInfo(Logger.CategoryType.WebSocket, $"Receive result is binary");
                    }
                }
            }

            Logger.LogInfo(Logger.CategoryType.WebSocket, "WaitForReceiveAsync exiting...");
        }

        internal void SetSessionToken(string sessionToken)
        {
            _sessionToken = sessionToken;
        }
    }
}