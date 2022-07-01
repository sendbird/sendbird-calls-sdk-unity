// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// %SendbirdCall
    /// </summary>
    public static class SendbirdCall
    {
        /// <summary>
        /// The app Id of your SendbirdCalls application.
        /// </summary>
        /// <returns>Application Id.</returns>
        public static string AppId => SendbirdCallSdk.Instance.AppId;

        /// <summary>
        /// Returns the currently authenticated user.
        /// </summary>
        /// <returns>User that is currently authenticated. Returns null if no user exists.</returns>
        public static SbUser CurrentUser => SendbirdCallSdk.Instance.CurrentUser;

        /// <summary>
        /// Initializes %Sendbird Call.
        /// </summary>
        /// <param name="appId">Your own app Id from your dashboard.</param>
        /// <returns>Returns true if the initialization succeeded. Otherwise, false.</returns>
        public static bool Init(string appId)
        {
            return SendbirdCallSdk.Instance.Init(appId);
        }

        /// <summary>
        /// SendbirdCalls SDK Version.
        /// </summary>
        /// <returns>SDK version.</returns>
        public static string GetSdkVersion()
        {
            return SendbirdCallSdk.VERSION;
        }
        
        /// <summary>
        /// Authenticates user with user Id and access token that you generated at %Sendbird Dashboard.
        /// </summary>
        /// <param name="authenticateParams">that contains User Id, Access Token.</param>
        /// <param name="authenticateHandler">The handler to call when the authentication is complete.</param>
        public static void Authenticate(SbAuthenticateParams authenticateParams, SbAuthenticateHandler authenticateHandler)
        {
            SendbirdCallSdk.Instance.Authenticate(authenticateParams, authenticateHandler);
        }
        
        /// <summary>
        /// Deauthenticates user.
        /// </summary>
        /// <param name="completionHandler">Error Handler to be called after deauthenticate process is finished.</param>
        public static void Deauthenticate(SbCompletionHandler completionHandler)
        {
            SendbirdCallSdk.Instance.Deauthenticate(completionHandler);
        }

        /// <summary>
        /// Creates a query for room list with specified parameters.
        /// </summary>
        /// <param name="roomListQueryParams">RoomListQuery Params with options for creating query.</param>
        /// <returns></returns>
        public static SbRoomListQuery CreateRoomListQuery(SbRoomListQueryParams roomListQueryParams)
        {
            return SendbirdCallSdk.Instance.CreateRoomListQuery(roomListQueryParams);
        }

        /// <summary>
        /// Creates a room for group calls.
        /// </summary>
        /// <param name="roomParams">SbRoomParams for creating SbRoom.</param>
        /// <param name="roomHandler">A callback function that receives information about a room or an error from %Sendbird server.</param>
        public static void CreateRoom(SbRoomParams roomParams, SbRoomHandler roomHandler)
        {
            SendbirdCallSdk.Instance.CreateRoom(roomParams, roomHandler);
        }
        
        /// <summary>
        /// Gets a locally-cached room instance by room Id.
        /// </summary>
        /// <param name="roomId">room Id.</param>
        /// <returns>SbRoom object with the corresponding roomId.</returns>
        public static SbRoom GetCachedRoomById(string roomId)
        {
            return SendbirdCallSdk.Instance.GetCachedRoomById(roomId);
        }
        
        /// <summary>
        /// Fetches a room instance from %Sendbird server.
        /// </summary>
        /// <param name="roomId">room Id.</param>
        /// <param name="roomHandler">Callback to be called after get SbRoom object corresponding the Id or an error.</param>
        public static void FetchRoomById(string roomId, SbRoomHandler roomHandler)
        {
            SendbirdCallSdk.Instance.FetchRoomById(roomId, roomHandler);
        }
        
        /// <summary>
        /// Sets logger level.
        /// </summary>
        /// <param name="logLevel">Level of logger.</param>
        public static void SetLogLevel(SbLogLevel logLevel)
        {
            SendbirdCallSdk.Instance.SetLogLevel(logLevel);
        }
    }
}