// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    internal abstract class ParticipantManagerAbstract
    {
        internal SbRoom OwnedRoom { get; }

        internal ParticipantManagerAbstract(SbRoom ownedRoom)
        {
            OwnedRoom = ownedRoom;
        }

        internal virtual void OnLocalParticipantEntered(SbLocalParticipant localParticipant) { }
        internal virtual void OnLocalParticipantExited(SbLocalParticipant localParticipant) { }
        internal virtual void OnRemoteParticipantExited(SbRemoteParticipant remoteParticipant) { }
        internal virtual void OnRemoteParticipantConnected(SbRemoteParticipant remoteParticipant) { }

        protected bool TrySetLocalAudioEnable(PeerConnectionClient peerConnectionClient, bool audioEnable)
        {
            if (peerConnectionClient == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "TrySetAudioEnable PeerConnectionClient is null");
                return false;
            }

            bool isApplied = peerConnectionClient.TrySetAudioEnable(audioEnable);
            if (isApplied == false)
            {
                return false;
            }

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error != null && apiResponse is ParticipantAudioChangedApiCommand.Response audioChangedResponse)
                {
                    OwnedRoom?.LocalParticipant?.UpdateFromCommandObject(audioChangedResponse.meParticipantCommandObject);
                }
            }

            string ownedRoomId = OwnedRoom?.RoomId;
            string localParticipantId = OwnedRoom?.LocalParticipant?.ParticipantId;

            if (string.IsNullOrEmpty(ownedRoomId) || string.IsNullOrEmpty(localParticipantId))
            {
                Logger.LogWarning(Logger.CategoryType.Room, "TrySetAudioEnable room or localparticipantId is invalid");
                return false;
            }

            ParticipantAudioChangedApiCommand.Request request = new ParticipantAudioChangedApiCommand.Request(ownedRoomId, localParticipantId, audioEnable, ResultHandler);
            CommandRouter.Instance.Send(request);

            return true;
        }
        
        protected bool TrySetLocalVideoEnable(PeerConnectionClient peerConnectionClient, bool videoEnable)
        {
            if (peerConnectionClient == null)
            {
                Logger.LogWarning(Logger.CategoryType.Room, "TrySetVideoEnable PeerConnectionClient is null");
                return false;
            }

            bool isApplied = peerConnectionClient.TrySetVideoEnable(videoEnable);
            if (isApplied == false)
            {
                return false;
            }

            void ResultHandler(ApiResponseAbstract apiResponse, SbError error)
            {
                if (error != null && apiResponse is ParticipantAudioChangedApiCommand.Response audioChangedResponse)
                {
                    OwnedRoom?.LocalParticipant?.UpdateFromCommandObject(audioChangedResponse.meParticipantCommandObject);
                }
            }

            string ownedRoomId = OwnedRoom?.RoomId;
            string localParticipantId = OwnedRoom?.LocalParticipant?.ParticipantId;

            if (string.IsNullOrEmpty(ownedRoomId) || string.IsNullOrEmpty(localParticipantId))
            {
                Logger.LogWarning(Logger.CategoryType.Room, "TrySetVideoEnable room or localparticipantId is invalid");
                return false;
            }

            ParticipantVideoChangedApiCommand.Request request = new ParticipantVideoChangedApiCommand.Request(ownedRoomId, localParticipantId, videoEnable, ResultHandler);
            CommandRouter.Instance.Send(request);

            return true;
        }
    }
}