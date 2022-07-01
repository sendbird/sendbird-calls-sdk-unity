// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sendbird.Calls
{
    internal class RoomParticipantCollection
    {
        private static readonly ReadOnlyCollection<SbParticipantAbstract> EMPTY_READ_ONLY_PARTICIPANTS = new ReadOnlyCollection<SbParticipantAbstract>(new List<SbParticipantAbstract>(0));
        private static readonly ReadOnlyCollection<SbRemoteParticipant> EMPTY_READ_ONLY_REMOTE_PARTICIPANTS = new ReadOnlyCollection<SbRemoteParticipant>(new List<SbRemoteParticipant>(0));
       
        private readonly List<SbRemoteParticipant> _remoteParticipants = new List<SbRemoteParticipant>();
        private bool _isChangedParticipants;
        private SbLocalParticipant _localParticipant;
        private ReadOnlyCollection<SbParticipantAbstract> _readOnlyParticipants = EMPTY_READ_ONLY_PARTICIPANTS;
        private ReadOnlyCollection<SbRemoteParticipant> _readOnlyRemoteParticipants = EMPTY_READ_ONLY_REMOTE_PARTICIPANTS;
        
        internal void CreateOrUpdateLocalParticipant(ParticipantCommandObject participantCommandObject)
        {
            if (_localParticipant != null)
            {
                Logger.LogInfo(Logger.CategoryType.Room, "RoomParticipantCollection::CreateOrUpdateLocalParticipant Update local participant");
                _localParticipant.UpdateFromCommandObject(participantCommandObject);
            }
            else
            {
                Logger.LogInfo(Logger.CategoryType.Room, "RoomParticipantCollection::CreateOrUpdateLocalParticipant Create local participant");
                _localParticipant = new SbLocalParticipant(participantCommandObject);
                _isChangedParticipants = true;
            }
        }

        internal void OnLocalParticipantExited()
        {
            _localParticipant = null;
            _isChangedParticipants = true;
        }

        internal SbRemoteParticipant InsertRemoteParticipant(ParticipantCommandObject participantCommandObject)
        {
            SbRemoteParticipant remoteParticipant = FindRemoteParticipant(participantCommandObject.participantId);
            if (remoteParticipant != null)
            {
                Logger.LogInfo(Logger.CategoryType.Room, $"RoomParticipantCollection::InsertRemoteParticipant Update remote participant id:{participantCommandObject.participantId}");
                remoteParticipant.UpdateFromCommandObject(participantCommandObject);
                return remoteParticipant;
            }

            Logger.LogInfo(Logger.CategoryType.Room, $"RoomParticipantCollection::InsertRemoteParticipant Create remote participant id:{participantCommandObject.participantId}");
            remoteParticipant = new SbRemoteParticipant(participantCommandObject);
            _remoteParticipants.Add(remoteParticipant);
            _isChangedParticipants = true;

            return remoteParticipant;
        }

        internal SbRemoteParticipant FindRemoteParticipant(string participantId)
        {
            if (string.IsNullOrEmpty(participantId))
            {
                Logger.LogInfo(Logger.CategoryType.Room, "RoomParticipantCollection::FindRemoteParticipant Participant id is null or empty");
                return null;
            }

            SbRemoteParticipant remoteParticipant = _remoteParticipants.Find(participant => participant.ParticipantId.Equals(participantId));
            return remoteParticipant;
        }

        internal void RemoveRemoteParticipant(string participantId)
        {
            for (int index = _remoteParticipants.Count - 1; 0 <= index; index--)
                if (_remoteParticipants[index].ParticipantId.Equals(participantId))
                {
                    _remoteParticipants.RemoveAt(index);
                    _isChangedParticipants = true;
                }
        }

        internal void RemoveAllExitedRemoteParticipants()
        {
            for (int index = _remoteParticipants.Count - 1; 0 <= index; index--)
                if (_remoteParticipants[index].State == SbParticipantState.Exited)
                    _remoteParticipants.Remove(_remoteParticipants[index]);
        }

        internal void ForceChangeExitedStateToAllRemoteParticipants()
        {
            _remoteParticipants.ForEach(participant => { participant.ForceChangeState(SbParticipantState.Exited); });
        }

        internal void ClearAllParticipants()
        {
            _localParticipant = null;
            _remoteParticipants.Clear();
        }

        internal bool HasLocalParticipant()
        {
            return _localParticipant != null;
        }

        internal SbLocalParticipant GetLocalParticipant()
        {
            return _localParticipant;
        }

        internal ReadOnlyCollection<SbParticipantAbstract> GetParticipants()
        {
            CheckChangedAndNewReadOnlyParticipants();
            return _readOnlyParticipants;
        }

        internal ReadOnlyCollection<SbRemoteParticipant> GetRemoteParticipants()
        {
            CheckChangedAndNewReadOnlyParticipants();
            return _readOnlyRemoteParticipants;
        }

        private void CheckChangedAndNewReadOnlyParticipants()
        {
            if (_isChangedParticipants == false)
                return;

            _readOnlyRemoteParticipants = new ReadOnlyCollection<SbRemoteParticipant>(_remoteParticipants);

            List<SbParticipantAbstract> tempParticipants = new List<SbParticipantAbstract>(_remoteParticipants);
            if (_localParticipant != null)
                tempParticipants.Add(_localParticipant);

            _readOnlyParticipants = new ReadOnlyCollection<SbParticipantAbstract>(tempParticipants);
            _isChangedParticipants = false;
        }
    }
}