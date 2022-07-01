// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class RoomAliveApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            internal Request(string roomId, string localParticipantId, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants/{localParticipantId}/alive";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                ResultDelegate = resultDelegate;
            }
        }
    }
}