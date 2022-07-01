// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls
{
    internal class AudioMcuParticipantManager : ParticipantManagerAbstract, ParticipantManagerEventListener
    {
        private Endpoint _endpoint;
        internal AudioMcuParticipantManager(SbRoom ownedRoom) : base(ownedRoom) { }

        internal override void OnLocalParticipantEntered(SbLocalParticipant localParticipant)
        {
            Logger.LogInfo(Logger.CategoryType.Room, "AudioMcuParticipantManager::OnLocalParticipantEntered");

            if (_endpoint != null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "AudioMcuParticipantManager::OnLocalParticipantEntered already entered local participant");
                return;
            }

            _endpoint = new Endpoint(OwnedRoom, EndpointType.SendReceive, EndpointType.None, localParticipant.IsAudioEnabled, false);
            _endpoint.Connect();
            
            localParticipant.SetParticipantManagerEventListener(this);
        }

        internal override void OnLocalParticipantExited(SbLocalParticipant localParticipant)
        {
            Logger.LogInfo(Logger.CategoryType.Room, "AudioMcuParticipantManager::OnLocalParticipantExited");

            localParticipant.SetParticipantManagerEventListener(null);
            
            _endpoint?.Close(false);
            _endpoint = null;
        }

        bool ParticipantManagerEventListener.MuteLocalMicrophone(string participantId)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"MuteMicrophone participantId:{participantId}");
            if (OwnedRoom == null || OwnedRoom.LocalParticipant == null || OwnedRoom.LocalParticipant.ParticipantId.Equals(participantId) == false)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "MuteMicrophone local participant is null or requested by remote participant.");
                return false;
            }

            return TrySetLocalAudioEnable(_endpoint?.PeerConnectionClient, false);
        }

        bool ParticipantManagerEventListener.UnMuteLocalMicrophone(string participantId)
        {
            Logger.LogInfo(Logger.CategoryType.Room, $"UnMuteMicrophone participantId:{participantId}");
            if (OwnedRoom == null || OwnedRoom.LocalParticipant == null || OwnedRoom.LocalParticipant.ParticipantId.Equals(participantId) == false)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "UnMuteMicrophone local participant is null or requested by remote participant.");
                return false;
            }

            return TrySetLocalAudioEnable(_endpoint?.PeerConnectionClient, true);
        }

        void ParticipantManagerEventListener.SetVideoOutRawImage(string participantId, RawImage rawImage)
        {
            Logger.LogWarning(Logger.CategoryType.Room, $"SetVideoOutRawImage is ignore because audio only room");
        }
    }
}