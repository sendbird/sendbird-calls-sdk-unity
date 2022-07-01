// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal static class SbParticipantStateTypeExtension
    {
        private static string EnumToJsonPropertyName(this SbParticipantState participantState)
        {
            return NewtonsoftJsonExtension.EnumToJsonPropertyName(participantState);
        }

        internal static SbParticipantState JsonPropertyNameToType(string jsonPropertyName)
        {
            if (SbParticipantState.Connected.EnumToJsonPropertyName().Equals(jsonPropertyName))
            {
                return SbParticipantState.Connected;
            }
            else if (SbParticipantState.Entered.EnumToJsonPropertyName().Equals(jsonPropertyName))
            {
                return SbParticipantState.Entered;
            }

            return SbParticipantState.Exited;
        }
    }
}