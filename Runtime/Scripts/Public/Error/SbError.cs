// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// Custom Error class for SendbirdCalls.
    /// </summary>
    public partial class SbError
    {
        /// <summary>
        /// Error Code that represents the type of the error.
        /// </summary>
        public SbErrorCode ErrorCode { get; }
        /// <summary>
        /// Error message.
        /// </summary>
        public string ErrorMessage { get; }
    }
}