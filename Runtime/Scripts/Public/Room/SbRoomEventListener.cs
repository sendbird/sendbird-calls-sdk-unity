// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.ObjectModel;

namespace Sendbird.Calls
{
    /// <summary>
    /// An interface that contains the OnRemoteParticipantEntered() and OnRemoteParticipantExited(), and other callback methods to receive a participant's events while in the room.
    /// </summary>
    public interface SbRoomEventListener
    {
        /// <summary>
        /// Invoked when a remote participant has entered a room.
        /// </summary>
        /// <param name="remoteParticipant">who entered.</param>
        void OnRemoteParticipantEntered(SbRemoteParticipant remoteParticipant);
        /// <summary>
        /// Invoked when a remote participant has exited a room.
        /// </summary>
        /// <param name="remoteParticipant">who exited.</param>
        void OnRemoteParticipantExited(SbRemoteParticipant remoteParticipant);
        /// <summary>
        /// Invoked when a remote participant's audio settings has changed.
        /// </summary>
        /// <param name="remoteParticipant">who has changed audio settings.</param>
        void OnRemoteAudioSettingsChanged(SbRemoteParticipant remoteParticipant);
        /// <summary>
        /// Invoked when a remote participant updates custom items in the room.
        /// </summary>
        /// <param name="updatedKeys">Update keys.</param>
        void OnCustomItemsUpdated(ReadOnlyCollection<string> updatedKeys);
        /// <summary>
        /// Invoked when a remote participant deletes custom items in the room.
        /// </summary>
        /// <param name="deletedItemKeys">Delete keys.</param>
        void OnCustomItemsDeleted(ReadOnlyCollection<string> deletedItemKeys);
        /// <summary>
        /// Invoked when a room has been deleted by using Platform API.
        /// </summary>
        void OnDeleted();
        /// <summary>
        /// Invoked when an error occurs on Sendbird server while processing a request.
        /// </summary>
        /// <param name="error">SbError object.</param>
        /// <param name="participant">who is affected.</param>
        void OnError(SbError error, SbParticipantAbstract participant);
    }
}