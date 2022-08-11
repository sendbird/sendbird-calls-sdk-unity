// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;

namespace Sendbird.Calls
{
    internal class SendbirdCallSdk : SingletonAbstract<SendbirdCallSdk>
    {
        internal const string VERSION = "1.0.0-beta.1";

        private SendbirdCallSdk() { }
        internal string AppId { get; private set; } = null;
        internal string ClientId { get; } = Guid.NewGuid().ToString();
        internal SbUser CurrentUser { get; private set; }

        internal bool Init(string appId)
        {
            Logger.LogInfo(Logger.CategoryType.Common, $"Init appId:{appId}");

            if (string.IsNullOrEmpty(appId))
            {
                Logger.LogError(Logger.CategoryType.Common, SbErrorCodeExtension.ErrorCodeToString(SbErrorCode.InvalidParameterValue, "AppId"));
                return false;
            }
            
            try
            {
                Unity.WebRTC.WebRTC.Dispose();
                Unity.WebRTC.WebRTC.Initialize();
            }
            catch (Exception exception)
            {
                Logger.LogError(Logger.CategoryType.Rtc, exception.Message);
                return false;
            }

            AppId = appId;
            CommandRouter.Instance.Init(AppId);
            return true;
        }

        internal void Authenticate(SbAuthenticateParams authenticateParams, SbAuthenticateHandler authenticateHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Common, $"Authenticate UserId:{authenticateParams?.UserId}, AccessToken:{authenticateParams?.AccessToken}");

            if (authenticateParams == null)
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("AuthenticateParams");
                Logger.LogWarning(Logger.CategoryType.Common, $"SendbirdCallSdk::Authenticate {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => authenticateHandler?.Invoke(null, error));
                return;
            }

            if (string.IsNullOrEmpty(AppId))
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("AppId");
                Logger.LogWarning(Logger.CategoryType.Common, $"SendbirdCallSdk::Authenticate {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => authenticateHandler?.Invoke(null, error));
                return;
            }

            if (string.IsNullOrEmpty(authenticateParams.UserId))
            {
                SbError error = SbErrorCodeExtension.CreateInvalidParameterValueError("UseId");
                Logger.LogWarning(Logger.CategoryType.Common, $"SendbirdCallSdk::Authenticate {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => authenticateHandler?.Invoke(null, error));
                return;
            }

            if (CurrentUser != null && CurrentUser.UserId.Equals(authenticateParams.UserId) == false)
            {
                Deauthenticate((error) =>
                {
                    Init(AppId);
                    Authenticate(authenticateParams, authenticateHandler);
                });
                return;
            }

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error == null)
                {
                    if (apiResponse is AuthenticateApiCommand.Response authenticateResponseAbstract)
                    {
                        CurrentUser = new SbUser(authenticateResponseAbstract.userCommandObject);
                        CommandRouter.Instance.SetSessionToken(authenticateResponseAbstract.sessionToken);
                        authenticateHandler?.Invoke(CurrentUser, null);
                    }
                    else
                    {
                        authenticateHandler?.Invoke(null, new SbError(SbErrorCode.MalformedData));
                    }
                }
                else
                {
                    authenticateHandler?.Invoke(null, error);
                }
            }

            AuthenticateApiCommand.Request request = new AuthenticateApiCommand.Request(AppId, authenticateParams.UserId, authenticateParams.AccessToken, ResultHandler);
            CommandRouter.Instance.Send(request);
        }

        internal void Deauthenticate(SbCompletionHandler completionHandler)
        {
            Logger.LogInfo(Logger.CategoryType.Common, "DeAuthenticate");

            if (CurrentUser == null)
            {
                SbError error = new SbError(SbErrorCode.NotAuthenticated);
                Logger.LogWarning(Logger.CategoryType.Common, $"SendbirdCallSdk::Deauthenticate {error.ErrorMessage}");
                SendbirdCallGameObject.Instance.CallOnNextFrame(() => completionHandler?.Invoke(error));
                return;
            }

            CurrentUser = null;
            CommandRouter.Instance.Terminate();

            SendbirdCallGameObject.Instance.CallOnNextFrame(() => { completionHandler?.Invoke(null); });
        }

        internal SbRoomListQuery CreateRoomListQuery(SbRoomListQueryParams roomListQueryParams)
        {
            return RoomManager.Instance.CreateRoomListQuery(roomListQueryParams);
        }

        internal void CreateRoom(SbRoomParams roomParams, SbRoomHandler roomHandler)
        {
            RoomManager.Instance.CreateRoom(roomParams, roomHandler);
        }

        internal SbRoom GetCachedRoomById(string roomId)
        {
            return RoomManager.Instance.GetCachedRoomById(roomId);
        }

        internal void FetchRoomById(string roomId, SbRoomHandler roomHandler)
        {
            RoomManager.Instance.FetchRoomById(roomId, roomHandler);
        }
        
        internal void OnApplicationQuit()
        {
            RoomManager.Instance.OnApplicationQuit();
            CommandRouter.Instance.Terminate();
        }

        internal void SetLogLevel(SbLogLevel logLevel)
        {
            Logger.SetLogLevel(logLevel.ToLogLevelType());
        }
    }
}