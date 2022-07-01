// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls
{
    internal interface ParticipantManagerEventListener
    {
        bool MuteLocalMicrophone(string participantId);
        bool UnMuteLocalMicrophone(string participantId);
        void SetVideoOutRawImage(string participantId, RawImage rawImage);
    }
}