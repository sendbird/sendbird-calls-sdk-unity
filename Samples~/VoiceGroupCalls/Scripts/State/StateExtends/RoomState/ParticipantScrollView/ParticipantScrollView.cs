// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class ParticipantScrollView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private ParticipantScrollViewContent _participantScrollViewContentPrefab = null;
        [SerializeField] private Text _participantCountText = null;
        private readonly Dictionary<string, ParticipantScrollViewContent> _activeScrollContentsByParticipantId = new Dictionary<string, ParticipantScrollViewContent>();

        private void Awake()
        {
            if (_scrollRect == null) Debug.LogError("You need to set ScrollRect in ParticipantScrollView");
            if (_participantScrollViewContentPrefab == null) Debug.LogError("You need to set ParticipantScrollViewContentPrefab in ParticipantScrollView");
        }

        public void OnEnterRoom(SbRoom room)
        {
            if (room == null)
                return;

            if (room.LocalParticipant != null)
            {
                OnEnterParticipant(room.LocalParticipant);
            }

            foreach (SbRemoteParticipant remoteParticipant in room.RemoteParticipants)
            {
                OnEnterParticipant(remoteParticipant);
            }
        }

        public void OnExitRoom()
        {
            foreach (ParticipantScrollViewContent content in _activeScrollContentsByParticipantId.Values)
            {
                Destroy(content.gameObject);
            }

            _activeScrollContentsByParticipantId.Clear();
        }

        public void OnEnterParticipant(SbParticipantAbstract participant)
        {
            ParticipantScrollViewContent participantScrollViewContent = Instantiate(_participantScrollViewContentPrefab, transform);
            participantScrollViewContent.ResetFromSbRoom(participant);
            participantScrollViewContent.transform.SetParent(_scrollRect.content);
            _activeScrollContentsByParticipantId.Add(participant.ParticipantId, participantScrollViewContent);

            if (_participantCountText != null)
            {
                _participantCountText.text = $"Participants:({_activeScrollContentsByParticipantId.Count}/100)";
            }
        }

        public void OnExitParticipant(SbParticipantAbstract participant)
        {
            if (_activeScrollContentsByParticipantId.TryGetValue(participant.ParticipantId, out ParticipantScrollViewContent outScrollViewContent))
            {
                Destroy(outScrollViewContent.gameObject);
                _activeScrollContentsByParticipantId.Remove(participant.ParticipantId);
            }
        }

        public void OnParticipantAudioSettingsChanged(SbParticipantAbstract participant)
        {
            if (_activeScrollContentsByParticipantId.TryGetValue(participant.ParticipantId, out ParticipantScrollViewContent outScrollViewContent))
            {
                outScrollViewContent.OnAudioSettingsChanged(participant.IsAudioEnabled);
            }
        }
    }
}