// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;

namespace Sendbird.Calls
{
    internal abstract class ApiRequestAbstract
    {
        public Action<ApiResponseAbstract, SbError> ResultDelegate { get; protected set; }
        public string URL { get; protected set; }
        public string HttpMethod { get; protected set; }
        public string Payload { get; protected set; }
        public bool IsSessionTokenRequired { get; protected set; }
        public string ShortLivedToken { get; protected set; }
        public Type ResponseType { get; protected set; }

        public void InvokeResult(ApiResponseAbstract response, SbError error)
        {
            SendbirdCallGameObject.Instance.CallOnNextFrame(() => ResultDelegate?.Invoke(response, error));
        }
    }

    internal abstract class ApiResponseAbstract { }
}