// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    public partial class SbUser
    {
        internal SbUser(UserCommandObject userCommandObject)
        {
            ResetFromUserCommandObject(userCommandObject);
        }

        internal void ResetFromUserCommandObject(UserCommandObject userCommandObject)
        {
            UserId = userCommandObject.userId;
            Nickname = userCommandObject.nickname;
            ProfileURL = userCommandObject.profileURL;
            Metadata = userCommandObject.metadata;
            IsActive = userCommandObject.isActive;
        }
    }
}