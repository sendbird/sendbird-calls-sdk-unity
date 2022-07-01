// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sendbird.Calls
{
    internal sealed class ResponseErrorApiCommand
    {
        [Serializable]
        internal sealed class Response : ApiResponseAbstract
        {
            private const string JSON_KEY_ERROR = "error";
            private const string JSON_KEY_CODE = "code";

            [JsonProperty(JSON_KEY_CODE)] internal readonly int errorCode = (int)SbErrorCode.UnknownError;
            [JsonProperty(JSON_KEY_ERROR)] internal readonly bool isError = false;
            [JsonProperty("message")] internal readonly string errorMessage = null;
            [JsonProperty("request_id")] internal readonly string requestId = null;
            [JsonProperty("type")] internal readonly string commandType = null;

            internal bool IsError()
            {
                return isError;
            }

            internal SbErrorCode GetSbcErrorCode()
            {
                return (SbErrorCode)errorCode;
            }

            internal string GetMessage()
            {
                return errorMessage;
            }

            internal static Response TryConvertJsonToResponse(string jsonString)
            {
                if (string.IsNullOrEmpty(jsonString))
                {
                    return null;
                }

                JObject jObject = null;
                try
                {
                    jObject = JObject.Parse(jsonString);
                }
                catch (Exception)
                {
                    Logger.LogWarning(Logger.CategoryType.Api, $"ResponseErrorApiCommand::TryConvertJsonToResponse invalid format json:{jsonString}");
                }
                
                if (jObject == null || jObject.ContainsKey(JSON_KEY_ERROR) == false || jObject.ContainsKey(JSON_KEY_CODE) == false)
                {
                    return null;
                }
                
                return JsonConvert.DeserializeObject<Response>(jsonString);
            }
        }
    }
}