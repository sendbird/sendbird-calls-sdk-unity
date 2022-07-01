// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class RoomState : StateAbstract, SbRoomEventListener
    {
        [SerializeField] private ParticipantScrollView _participantScrollView = null;
        [SerializeField] private Toggle _localParticipantAudioToggle = null;
        [SerializeField] private Button _exitButton = null;
        [SerializeField] private Button _customItemsButton = null;
        [SerializeField] private CustomItemsPopup _customItemsPopup = null;

        private SbRoom _sbRoom = null;

        private void Awake()
        {
            if (_localParticipantAudioToggle == null) Debug.LogError("You need to set LocalParticipantAudioToggle in room state");
            if (_participantScrollView == null) Debug.LogError("You need to set ParticipantScrollView in room state");
            if (_exitButton == null) Debug.LogError("You need to set ExitButton in room state");
            if (_customItemsButton == null) Debug.LogError("You need to set CustomItemButton in room state");
            if (_customItemsPopup == null) Debug.LogError("You need to set CustomItemPopup in room state");

            _localParticipantAudioToggle.onValueChanged.AddListener(OnChangeLocalParticipantAudioOnOffToggle);
            _exitButton.onClick.AddListener(OnClickExitButton);
            _customItemsButton.onClick.AddListener(OnClickCustomItemsButton);
        }

        public override StateType GetStateType()
        {
            return StateType.Room;
        }

        public override void OnOpenState()
        {
            _participantScrollView.gameObject.SetActive(true);
            _customItemsPopup.Close();
        }

        public void EnterRoom(SbRoom room)
        {
            if (room == null)
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup("RoomState::SetSbRoom room is null");
                return;
            }

            _sbRoom = room;
            _sbRoom.AddEventListener(this);

            _participantScrollView.OnEnterRoom(_sbRoom);

            bool isMuteAudio = !room.LocalParticipant.IsAudioEnabled;
            _localParticipantAudioToggle.SetIsOnWithoutNotify(isMuteAudio);
        }

        private void ExitRoom()
        {
            _participantScrollView.OnExitRoom();
            _sbRoom.RemoveEventListener(this);
            _sbRoom = null;
            SampleGroupCallMain.Instance.OpenState(StateType.RoomList);
        }

        private void OnClickExitButton()
        {
            _sbRoom.Exit();
            ExitRoom();
        }

        private void OnChangeLocalParticipantAudioOnOffToggle(bool isMuteAudio)
        {
            if (isMuteAudio)
            {
                _sbRoom.LocalParticipant.MuteMicrophone();
            }
            else
            {
                _sbRoom.LocalParticipant.UnmuteMicrophone();
            }

            _participantScrollView.OnParticipantAudioSettingsChanged(_sbRoom.LocalParticipant);
        }

        private void OnClickCustomItemsButton()
        {
            _customItemsPopup.Open(_sbRoom);
        }

        void SbRoomEventListener.OnRemoteParticipantEntered(SbRemoteParticipant remoteParticipant)
        {
            _participantScrollView.OnEnterParticipant(remoteParticipant);
        }

        void SbRoomEventListener.OnRemoteParticipantExited(SbRemoteParticipant remoteParticipant)
        {
            _participantScrollView.OnExitParticipant(remoteParticipant);
        }

        void SbRoomEventListener.OnRemoteAudioSettingsChanged(SbRemoteParticipant remoteParticipant)
        {
            _participantScrollView.OnParticipantAudioSettingsChanged(remoteParticipant);
        }

        void SbRoomEventListener.OnCustomItemsUpdated(ReadOnlyCollection<string> updatedKeys)
        {
            Debug.Log($"RoomState::OnCustomItemsUpdated");
        }

        void SbRoomEventListener.OnCustomItemsDeleted(ReadOnlyCollection<string> deletedItemKeys)
        {
            Debug.Log($"RoomState::OnCustomItemsDeleted");
        }

        void SbRoomEventListener.OnDeleted()
        {
            ExitRoom();
        }

        void SbRoomEventListener.OnError(SbError error, SbParticipantAbstract participant)
        {
            SampleGroupCallMain.Instance.OpenNotifyPopup($"RoomState::OnError ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");

            if (participant is SbLocalParticipant)
            {
                ExitRoom();
            }
            else
            {
                _participantScrollView.OnExitParticipant(participant);
            }
        }
    }
}