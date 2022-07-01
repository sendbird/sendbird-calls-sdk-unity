// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.ObjectModel;

namespace Sendbird.Calls
{
    /// <summary>
    /// A callback for when the action is completed.
    /// </summary>
    /// <param name="error">null if no error.</param>
    public delegate void SbCompletionHandler(SbError error);

    /// <summary>
    /// Returns a SbUser and an error based on the results of the call to SendbirdCall::Authenticate().
    /// </summary>
    /// <param name="sbUser">An authenticated user.</param>
    /// <param name="error">An error that could occur while authenticating.</param>
    public delegate void SbAuthenticateHandler(SbUser sbUser, SbError error);

    /// <summary>
    /// A callback function that receives information about a list of rooms or an error from %Sendbird server.
    /// </summary>
    /// <param name="rooms">Retrieved rooms. If an error occurred, the list will be empty.</param>
    /// <param name="error">If an error occurred, it will be not-null. Otherwise, null.</param>
    public delegate void SbRoomListQueryHandler(ReadOnlyCollection<SbRoom> rooms, SbError error);

    /// <summary>
    /// A callback function that receives information about a room or an error from %Sendbird server.
    /// </summary>
    /// <param name="room">If the request is successful, it will not be null. Otherwise, it will be null.</param>
    /// <param name="error">If the request is failed, it will not be null. Otherwise, it will be null.</param>
    public delegate void SbRoomHandler(SbRoom room, SbError error);

    /// <summary>
    /// Returns a customItems and an error after a call to get the CustomItems.
    /// </summary>
    /// <param name="customItems">Key-value dictionary of the updated custom items.</param>
    /// <param name="error">An error that could occur while changing `CustomItems`.</param>
    public delegate void SbRoomFetchCustomItemsHandler(ReadOnlyDictionary<string, string> customItems, SbError error);

    /// <summary>
    /// Returns a customItems, an affectedKeys, and an error after a call to modify the CustomItems.
    /// </summary>
    /// <param name="customItems">Key-value dictionary of the updated custom items.</param>
    /// <param name="affectedKeys">Array of String keys that are modified as a result of the function call.</param>
    /// <param name="error">An error that could occur while changing `CustomItems`.</param>
    public delegate void SbRoomCustomItemsHandler(ReadOnlyDictionary<string, string> customItems, ReadOnlyCollection<string> affectedKeys, SbError error);
}