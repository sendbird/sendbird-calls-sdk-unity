// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    public abstract partial class SbParticipantAbstract
    {
        internal string ClientId { get; private set; }
        internal ParticipantManagerEventListener ParticipantManagerEventListener { get; private set; } = null;

        internal SbParticipantAbstract(ParticipantCommandObject participantCommandObject)
        {
            User = new SbUser(participantCommandObject.userCommandObject);
            UpdateFromCommandObject(participantCommandObject);
        }

        internal void UpdateFromCommandObject(ParticipantCommandObject participantCommandObject)
        {
            ParticipantId = participantCommandObject.participantId;
            EnteredAt = participantCommandObject.enteredAt;
            UpdatedAt = participantCommandObject.updatedAt;
            ExitAt = participantCommandObject.exitAt;
            Duration = participantCommandObject.duration;
            ClientId = participantCommandObject.clientId;
            IsAudioEnabled = participantCommandObject.isAudioOn;
            State = SbParticipantStateTypeExtension.JsonPropertyNameToType(participantCommandObject.state);

            User?.ResetFromUserCommandObject(participantCommandObject.userCommandObject);
        }

        internal void ForceChangeState(SbParticipantState participantState)
        {
            State = participantState;
        }

        internal void SetParticipantManagerEventListener(ParticipantManagerEventListener participantManagerEventListener)
        {
            ParticipantManagerEventListener = participantManagerEventListener;
        }
    }
}