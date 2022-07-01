// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal class ApiClient
    {
        internal static readonly string HEADER_USER_AGENT = $"calls-unity/{SendbirdCallSdk.VERSION}";
        private static readonly string HEADER_SENDBIRD = $"unity,{Application.unityVersion},{SendbirdCallSdk.VERSION}";
        private readonly StringBuilder _urlStringBuilder = new StringBuilder();

        private string _baseUrl = null;
        private string _sessionToken = null;

        internal void Init(string appId)
        {
            _baseUrl = $"https://api-{appId}.calls.sendbird.com";
        }

        internal void Terminate()
        {
            SetSessionToken(null);
        }

        internal void Send(ApiRequestAbstract apiRequest)
        {
            SendbirdCallGameObject.Instance.StartCoroutine(SendCoroutine(apiRequest));
        }

        private IEnumerator SendCoroutine(ApiRequestAbstract apiRequest)
        {
            if (string.IsNullOrEmpty(apiRequest?.URL))
            {
                Logger.LogWarning(Logger.CategoryType.Api, $"Send invalid type:{apiRequest?.GetType()}");
                SbErrorCode errorCode = SbErrorCode.InvalidParameterValue;
                string errorDescription = SbErrorCodeExtension.ErrorCodeToString(errorCode, "HTTP URL");
                apiRequest?.InvokeResult(null, new SbError(errorCode, errorDescription));
                yield break;
            }

            if (apiRequest.IsSessionTokenRequired && string.IsNullOrEmpty(_sessionToken))
            {
                Logger.LogWarning(Logger.CategoryType.Api, $"Send invalid session token type:{apiRequest.GetType()} token:{_sessionToken}");
                apiRequest.InvokeResult(null, new SbError(SbErrorCode.NotAuthenticated));
                yield break;
            }

            _urlStringBuilder.Clear();
            _urlStringBuilder.Append(_baseUrl);
            _urlStringBuilder.Append("/");
            _urlStringBuilder.Append(apiRequest.URL);

            string payload = apiRequest.Payload;
            Logger.LogInfo(Logger.CategoryType.Api, $"Request type:{apiRequest.GetType()}\n url:{_urlStringBuilder}\n payload:{payload}");
            
            int autoRetryCountIfNetworkError = 0;
            
            SEND_WEB_REQUEST_START_LABEL:
            using (UnityWebRequest webRequest = new UnityWebRequest(_urlStringBuilder.ToString(), apiRequest.HttpMethod))
            {
                if (string.IsNullOrEmpty(payload) == false)
                {
                    byte[] contents = Encoding.UTF8.GetBytes(payload);
                    webRequest.uploadHandler = new UploadHandlerRaw(contents);
                }

                webRequest.downloadHandler = new DownloadHandlerBuffer();

                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("User-Agent", HEADER_USER_AGENT);
                webRequest.SetRequestHeader("SendBird", HEADER_SENDBIRD);
                if (string.IsNullOrEmpty(_sessionToken) == false)
                {
                    webRequest.SetRequestHeader("SBCall-Session-Token", _sessionToken);
                }

                if (string.IsNullOrEmpty(SendbirdCallSdk.Instance.ClientId) == false)
                {
                    webRequest.SetRequestHeader("SBCall-Client-Id", SendbirdCallSdk.Instance.ClientId);
                }

                webRequest.timeout = 3;
                
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    Logger.LogWarning(Logger.CategoryType.Api, $"Response type:{apiRequest.GetType()}\n isNetworkError:{webRequest.isNetworkError} error:{webRequest.error}");

                    const int AUTO_RETRY_MAX = 3;
                    autoRetryCountIfNetworkError++;
                    if (autoRetryCountIfNetworkError <= AUTO_RETRY_MAX)
                    {
                        goto SEND_WEB_REQUEST_START_LABEL;
                    }
                    
                    apiRequest.InvokeResult(null, new SbError(SbErrorCode.RequestFailed, webRequest.error));
                    yield break;
                }
                
                string jsonString = string.Empty;
                if (webRequest.downloadHandler != null)
                    jsonString = webRequest.downloadHandler.text;

                ResponseErrorApiCommand.Response errorCommand = ResponseErrorApiCommand.Response.TryConvertJsonToResponse(jsonString);
                if (errorCommand != null && errorCommand.IsError())
                {
                    Logger.LogWarning(Logger.CategoryType.Api, $"Response type:{apiRequest.GetType()}\n ErrorCode:{errorCommand.GetSbcErrorCode()}\n ErrorMessage:{errorCommand.GetMessage()}");
                    apiRequest.InvokeResult(null, new SbError(errorCommand.GetSbcErrorCode(), errorCommand.GetMessage()));
                    yield break;
                }
                
                if (string.IsNullOrEmpty(webRequest.error) == false)
                {
                    Logger.LogWarning(Logger.CategoryType.Api, $"Response type:{apiRequest.GetType()}\n error:{webRequest.error}\n text:{jsonString}");
                    apiRequest.InvokeResult(null, new SbError(SbErrorCode.RequestFailed, webRequest.error));
                    yield break;
                }

                Logger.LogInfo(Logger.CategoryType.Api, $"Response type:{apiRequest.GetType()}\n text:{jsonString}");

                ApiResponseAbstract responseAbstract = null;
                Type responseType = apiRequest.ResponseType;
                if (responseType != null)
                {
                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                    responseAbstract = JsonConvert.DeserializeObject(jsonString, responseType, jsonSerializerSettings) as ApiResponseAbstract;

                    if (responseAbstract == null)
                    {
                        apiRequest.InvokeResult(null, new SbError(SbErrorCode.WrongResponse, $"Parsed command ${responseType.Name} is not a ApiResponse."));
                        yield break;
                    }
                }

                apiRequest.InvokeResult(responseAbstract, null);
            }
        }

        internal void SetSessionToken(string sessionToken)
        {
            _sessionToken = sessionToken;
        }
    }
}