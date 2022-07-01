// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal static class SbErrorCodeExtension
    {
        internal static string ErrorCodeToString(SbErrorCode inErrorCode, string inOptionValue = "")
        {
            switch (inErrorCode)
            {
                case SbErrorCode.ClientAlreadyEntered:              return "The client has already entered the room.";
                case SbErrorCode.EnteringRoomStillInProgress:       return "The participant is trying to enter the room while the previous request is still in progress.";
                case SbErrorCode.LocalParticipantLostConnection:    return "The local participant exited the room due to lost connection.";
                case SbErrorCode.RequestFailed:                     return "The HTTP request failed.";
                case SbErrorCode.WrongResponse:                     return "The response contains an unexpected object type of data.";
                case SbErrorCode.QueryInProgress:                   return "The previous query is still in progress.";
                case SbErrorCode.MalformedData:                     return "The data format of the response is invalid.";
                case SbErrorCode.InvalidParameterValue:             return $"{inOptionValue} is an invalid or empty value.";
                case SbErrorCode.NotAuthenticated:                  return "The user is not authenticated.";
                case SbErrorCode.InvalidParams:                     return "The request specifies one or more invalid parameters.";
                case SbErrorCode.MissingParams:                     return "The request is missing one or more required parameters.";
                case SbErrorCode.NotFound:                          return "The requested resource cannot be found.";
                case SbErrorCode.UniqueConstraint:                  return "The requested resource already exists.";
                case SbErrorCode.FreePlanEnded:                     return "The request cannot be completed because your free plan ended.";
                case SbErrorCode.NotAllowed:                        return "The request is not allowed to perform this action.";
                case SbErrorCode.NotAuthorized:                     return "The request is not authorized to perform this operation.";
                case SbErrorCode.ServerInternalError:               return "The server failed to process the request due to an internal reason.";
                case SbErrorCode.ParticipantsLimitExceededInRoom:   return "";
                case SbErrorCode.InvalidParticipantId:              return "";
                case SbErrorCode.ClientIdAlreadyExists:             return "";
                case SbErrorCode.InvalidRequest:                    return "";
                case SbErrorCode.EndpointToSendStreamAlreadyExists: return "";
                case SbErrorCode.RoomDeleted:                       return "The request cannot be processed because the room ID doesn't exist or was deleted.";
                case SbErrorCode.UnknownError:                      return "The server encounters an unexpected exception while trying to process the request.";
                default:                                            return string.Empty;
            }
        }

        internal static SbError CreateInvalidParameterValueError(string inParamName)
        {
            return new SbError(SbErrorCode.InvalidParameterValue, ErrorCodeToString(SbErrorCode.InvalidParameterValue, inParamName));
        }
    }
}