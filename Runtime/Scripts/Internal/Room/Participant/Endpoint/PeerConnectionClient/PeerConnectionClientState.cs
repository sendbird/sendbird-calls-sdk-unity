// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

internal enum PeerConnectionClientState
{
    Idle,
    Offering,
    Answering,
    RestartOffering,
    RestartAnswering,
    Reconnecting,
    ReconnectionFailed,
    Connected,
    Closed
}