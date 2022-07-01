// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.ObjectModel;

namespace Sendbird.Calls
{
    /// <summary>
    /// Class for SendbirdCalls User.
    /// </summary>
    public partial class SbUser
    {
        /// <summary>
        /// The user ID of the call user.
        /// </summary>
        public string UserId { get; private set; }
        /// <summary>
        /// The nickname of the user.
        /// </summary>
        public string Nickname { get; private set; }
        /// <summary>
        /// The profile image URL of the user.
        /// </summary>
        public string ProfileURL { get; private set; }
        /// <summary>
        /// Metadata of the user.
        /// </summary>
        public ReadOnlyDictionary<string, string> Metadata { get; private set; }
        /// <summary>
        /// Activity status of the user. If it is `false`, the user is offline. The default value is `false`.
        /// </summary>
        public bool IsActive { get; private set; }
    }
}