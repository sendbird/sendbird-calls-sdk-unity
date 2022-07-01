// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using Unity.WebRTC;

namespace Sendbird.Calls
{
    internal interface PeerConnectionClientEventListener
    {
        void OnRTCSetLocalDescription(string inSdp);
        void OnPeerConnectionClientConnected();
        void OnPeerConnectionClientReconnectionFailed();
        void OnPeerConnectionClientError(string errorMessage);
        void OnPeerConnectionClientClosed();
    }
}