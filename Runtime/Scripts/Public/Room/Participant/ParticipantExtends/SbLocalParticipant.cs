// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// A class that contains all operational methods of a local participant to handle their audio and video.
    /// </summary>
    public partial class SbLocalParticipant
    {
        /// <summary>
        /// Mutes the local user's audio.
        /// </summary>
        public void MuteMicrophone()
        {
            if (ParticipantManagerEventListener == null) return;

            if (ParticipantManagerEventListener.MuteLocalMicrophone(ParticipantId))
            {
                IsAudioEnabled = false;
            }
        }

        /// <summary>
        /// UnMutes the local user's audio.
        /// </summary>
        public void UnmuteMicrophone() 
        {  
            if (ParticipantManagerEventListener == null) return;

            if (ParticipantManagerEventListener.UnMuteLocalMicrophone(ParticipantId))
            {
                IsAudioEnabled = true;
            }
        }
    }
}