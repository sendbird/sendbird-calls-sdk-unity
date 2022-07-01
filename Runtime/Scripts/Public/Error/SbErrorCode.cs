// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// Custom Error codes representing different error scenarios.
    /// </summary>
    public enum SbErrorCode
    {
        // Client
        /// <summary>
        /// The HTTP request failed.
        /// </summary>
        RequestFailed /*-----------------------------*/ = 1800200,
        /// <summary>
        /// The response contains an unexpected object type of data.
        /// </summary>
        WrongResponse /*-----------------------------*/ = 1800205,
        /// <summary>
        /// The previous query is still in progress.
        /// </summary>
        QueryInProgress /*---------------------------*/ = 1800206,
        /// <summary>
        /// The data format of the response is invalid.
        /// </summary>
        MalformedData /*-----------------------------*/ = 1800208,

        // General
        /// <summary>
        /// Invalid or empty value.
        /// </summary>
        InvalidParameterValue /*---------------------*/ = 1800300,
        /// <summary>
        /// 1800303, The user is not authenticated.
        /// </summary>
        NotAuthenticated /*--------------------------*/ = 1800303,

        // Room
        /// <summary>
        /// The client has already entered the room.
        /// </summary>
        ClientAlreadyEntered /*----------------------*/ = 1800700,
        /// <summary>
        /// The participant is trying to enter the room while the previous request is still in progress.
        /// </summary>
        EnteringRoomStillInProgress /*---------------*/ = 1800701,
        /// <summary>
        /// The local participant exited the room due to lost connection.
        /// </summary>
        LocalParticipantLostConnection /*------------*/ = 1800706,

        // Server
        /// <summary>
        /// The request specifies one or more invalid parameters.
        /// </summary>
        InvalidParams /*-----------------------------*/ = 400100,
        /// <summary>
        /// The request is missing one or more required parameters.
        /// </summary>
        MissingParams /*-----------------------------*/ = 400111,
        /// <summary>
        /// The requested resource cannot be found.
        /// </summary>
        NotFound /*----------------------------------*/ = 400200,
        /// <summary>
        /// The requested resource already exists.
        /// </summary>
        UniqueConstraint /*--------------------------*/ = 400201,
        /// <summary>
        /// The request cannot be completed because your free plan ended.
        /// </summary>
        FreePlanEnded /*-----------------------------*/ = 400800,
        /// <summary>
        /// The request is not allowed to perform this action.
        /// </summary>
        NotAllowed /*--------------------------------*/ = 401120,
        /// <summary>
        /// The request is not authorized to perform this operation.
        /// </summary>
        NotAuthorized /*-----------------------------*/ = 401121,
        /// <summary>
        /// The server failed to process the request due to an internal reason.
        /// </summary>
        ServerInternalError /*-----------------------*/ = 1400999,
        ParticipantsLimitExceededInRoom /*-----------*/ = 1400120,
        InvalidParticipantId /*----------------------*/ = 1400121,
        ClientIdAlreadyExists /*---------------------*/ = 1400122,
        InvalidRequest /*----------------------------*/ = 1400123,
        EndpointToSendStreamAlreadyExists /*---------*/ = 1400124,
        /// <summary>
        /// The request cannot be processed because the room ID doesn't exist or was deleted.
        /// </summary>
        RoomDeleted /*-------------------------------*/ = 1400126,
        /// <summary>
        /// The server encounters an unexpected exception while trying to process the request.
        /// </summary>
        UnknownError /*------------------------------*/ = 500999
    }
}