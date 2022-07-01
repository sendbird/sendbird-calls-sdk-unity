// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class ParticipantScrollViewContent : MonoBehaviour
    {
        [SerializeField] private Image _bgImage = null;
        [SerializeField] private Text _userNicknameOrIdText = null;
        [SerializeField] private Text _participantIdText = null;
        [SerializeField] private GameObject _muteIconGameObject = null;
        private SbParticipantAbstract _sbParticipant = null;

        public void ResetFromSbRoom(SbParticipantAbstract participant)
        {
            _sbParticipant = participant;
            if (_sbParticipant == null)
                return;

            if (_userNicknameOrIdText != null)
            {
                _userNicknameOrIdText.text = string.IsNullOrEmpty(_sbParticipant.User?.Nickname) == false ? _sbParticipant.User?.Nickname : _sbParticipant.User?.UserId;
            }

            if (_participantIdText != null)
            {
                _participantIdText.text = _sbParticipant.ParticipantId;
            }

            if (_bgImage != null)
            {
                _bgImage.color = participant is SbLocalParticipant ? Color.white : new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }

            OnAudioSettingsChanged(participant.IsAudioEnabled);
        }

        public void OnAudioSettingsChanged(bool isAudioEnabled)
        {
            if (_muteIconGameObject != null)
            {
                _muteIconGameObject.SetActive(!isAudioEnabled);
            }
        }
    }
}