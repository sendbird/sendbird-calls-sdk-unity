// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    public partial class SbError
    {
        internal SbError(SbErrorCode inErrorCode, string inErrorMessage = "")
        {
            ErrorCode = inErrorCode;
            ErrorMessage = inErrorMessage;
            if (string.IsNullOrEmpty(ErrorMessage)) ErrorMessage = SbErrorCodeExtension.ErrorCodeToString(ErrorCode);
        }
    }
}