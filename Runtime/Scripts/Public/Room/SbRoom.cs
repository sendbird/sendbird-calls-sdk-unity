// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sendbird.Calls
{
    /// <summary>
    /// A class that provides the Enter(), Exit(), and other methods, which handle information about the room and operate with other types of objects such as a participant.
    /// </summary>
    public partial class SbRoom
    {
        /// <summary>
        /// Returns a room Id.
        /// </summary>
        public string RoomId { get; }
        /// <summary>
        /// The timestamp of when the room was created, in Unix milliseconds.
        /// </summary>
        public long CreatedAt { get; }
        /// <summary>
        /// The ID of a user who created a room.
        /// </summary>
        public string CreatedBy { get; }
        /// <summary>
        /// The local participant in a room.
        /// </summary>
        public SbLocalParticipant LocalParticipant => _participantCollection.GetLocalParticipant();
        /// <summary>
        /// The list of all participants in a room.
        /// </summary>
        public ReadOnlyCollection<SbParticipantAbstract> Participants => _participantCollection.GetParticipants();
        /// <summary>
        /// The list of remote participants in a room.
        /// </summary>
        public ReadOnlyCollection<SbRemoteParticipant> RemoteParticipants => _participantCollection.GetRemoteParticipants();
        /// <summary>
        /// The state of a room. Valid values are SbRoomState::Open and SbRoomState::Deleted.
        /// </summary>
        public SbRoomState State { get; private set; }
        /// <summary>
        /// Indicates the room type as audio or video(Not supported yet) and the capacity of a room.
        /// </summary>
        public SbRoomType RoomType { get; }
        /// <summary>
        /// Custom items for this room.
        /// </summary>
        public ReadOnlyDictionary<string, string> CustomItems { get; private set; } = EMPTY_READ_ONLY_CUSTOM_ITEMS;

        /// <summary>
        /// Enters a room. The participant's audio or video(Not supported yet) can be configured with `RoomEnterParams` when entering.
        /// </summary>
        /// <param name="roomEnterParams"></param>
        /// <param name="completionHandler"></param>
        public void Enter(SbRoomEnterParams roomEnterParams, SbCompletionHandler completionHandler)
        {
            Enter_Internal(roomEnterParams, completionHandler);
        }

        /// <summary>
        /// Exits a room.
        /// </summary>
        public void Exit()
        {
            Exit_Internal();
        }

        /// <summary>
        /// Get latest custom items for this room.
        /// </summary>
        /// <param name="roomFetchCustomItemsHandler"></param>
        public void FetchCustomItems(SbRoomFetchCustomItemsHandler roomFetchCustomItemsHandler)
        {
            FetchCustomItems_Internal(roomFetchCustomItemsHandler);
        }

        /// <summary>
        /// Updates custom items for this room.
        /// </summary>
        /// <param name="customItems">Custom items (String dictionary) to be updated or inserted.</param>
        /// <param name="roomCustomItemsHandler">Callback completionHandler. Contains custom items, changes custom items, and error.</param>
        public void UpdateCustomItems(Dictionary<string, string> customItems, SbRoomCustomItemsHandler roomCustomItemsHandler)
        {
            UpdateCustomItems_Internal(customItems, roomCustomItemsHandler);
        }

        /// <summary>
        /// Deletes custom items of the call.
        /// </summary>
        /// <param name="customKeys">Keys of the custom item that you want to delete.</param>
        /// <param name="roomCustomItemsHandler">Callback completionHandler. Contains custom items, changes custom items, and error.</param>
        public void DeleteCustomItems(List<string> customKeys, SbRoomCustomItemsHandler roomCustomItemsHandler)
        {
            DeleteCustomItems_Internal(customKeys, roomCustomItemsHandler);
        }

        /// <summary>
        /// Adds a listener to receive events about a room.
        /// </summary>
        /// <param name="roomEventListener">An identifier of given SbRoomEventListener.</param>
        public void AddEventListener(SbRoomEventListener roomEventListener)
        {
            if (_roomEventListeners.Contains(roomEventListener) == false) _roomEventListeners.Add(roomEventListener);
        }

        /// <summary>
        /// Removes a listener to stop receiving events about a room.
        /// </summary>
        /// <param name="roomEventListener">An identifier that was passed to Room via AddEventListener().</param>
        public void RemoveEventListener(SbRoomEventListener roomEventListener)
        {
            if (_roomEventListeners.Contains(roomEventListener)) _roomEventListeners.Remove(roomEventListener);
        }

        /// <summary>
        /// Removes all listeners to stop receiving events about a room.
        /// </summary>
        public void RemoveAllEventListeners()
        {
            _roomEventListeners.Clear();
        }
    }
}