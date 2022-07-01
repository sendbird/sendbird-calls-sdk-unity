// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// A class that provides information about a participant and methods to set a participant's audio.
    /// </summary>
    public abstract partial class SbParticipantAbstract
    {
        /// <summary>
        ///  A unique identifier for a participant in a room.
        /// </summary>
        public string ParticipantId { get; private set; }
        /// <summary>
        /// The timestamp of when the participant enter the room, in Unix milliseconds.
        /// </summary>
        public long EnteredAt { get; private set; }
        /// <summary>
        /// The timestamp of when the participant information was updated within the room, in Unix milliseconds.
        /// </summary>
        public long UpdatedAt { get; private set; }
        /// <summary>
        /// The timestamp of when the participant exited the room, in Unix milliseconds. If the value is 0, it means the participant is present in the room.
        /// </summary>
        public long ExitAt { get; private set; }
        /// <summary>
        /// The period from the time when the participant entered the room to the time the participant left the room, measured in seconds. If the value is 0, it means the participant is present in the room.
        /// </summary>
        public long Duration { get; private set; }
        /// <summary>
        /// The state of the participant. Valid values are SbParticipantState::Entered, SbParticipantState::Exited, and SbParticipantState::Connected.
        /// </summary>
        public SbParticipantState State { get; private set; }
        /// <summary>
        /// Indicates whether the participant has enabled their audio.
        /// </summary>
        public bool IsAudioEnabled { get; protected set; }
        /// <summary>
        /// Indicates a user in Calls who corresponds to the participant.
        /// </summary>
        public SbUser User { get; }
    }
}