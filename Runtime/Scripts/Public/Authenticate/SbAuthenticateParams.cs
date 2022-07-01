// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// Configuration for authentication of Sendbird user.
    /// </summary>
    public class SbAuthenticateParams
    {
        /// <summary>
        /// Access Token used for extra layer of security.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// User Id of the user.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// SbAuthenticateParams constructor.
        /// </summary>
        /// <param name="userId">User Id of the user.</param>
        public SbAuthenticateParams(string userId)
        {
            UserId = userId;
        }
    }
}