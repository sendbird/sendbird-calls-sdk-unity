// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using UnityEngine.Networking;

namespace Sendbird.Calls
{
    internal sealed class RoomConnectedApiCommand
    {
        internal sealed class Request : ApiRequestAbstract
        {
            internal Request(string roomId, string localParticipantId, string endpointId, Action<ApiResponseAbstract, SbError> resultDelegate)
            {
                URL = $"v1/rooms/{roomId}/participants/{localParticipantId}/endpoints/{endpointId}/connected";
                HttpMethod = UnityWebRequest.kHttpVerbPOST;
                ResultDelegate = resultDelegate;
            }
        }
    }
}