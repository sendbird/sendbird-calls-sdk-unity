// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class RoomListScrollViewContent : MonoBehaviour
    {
        [SerializeField] private Button _enterRoomButton = null;
        [SerializeField] private Text _roomIdText = null;
        [SerializeField] private Text _roomTypeText = null;
        [SerializeField] private Text _roomStateText = null;
        [SerializeField] private Text _createdAtText = null;
        [SerializeField] private Text _createdByText = null;
        [SerializeField] private Text _participantsCountText = null;
        private SbRoom _sbRoom = null;

        private void Awake()
        {
            if (_enterRoomButton == null)
            {
                Debug.LogError("You need to set EnterRoomButton in RoomListScrollViewContent");
            }
            else
            {
                _enterRoomButton.onClick.AddListener(OnClickEnterRoomButton);
            }
        }

        public void ResetFromSbRoom(SbRoom room)
        {
            _sbRoom = room;
            if (_sbRoom == null)
                return;

            if (_roomIdText != null) _roomIdText.text = _sbRoom.RoomId;
            if (_roomTypeText != null) _roomTypeText.text = _sbRoom.RoomType.ToString();
            if (_roomStateText != null) _roomStateText.text = _sbRoom.State.ToString();
            if (_createdByText != null) _createdByText.text = _sbRoom.CreatedBy;
            if (_participantsCountText != null) _participantsCountText.text = _sbRoom.RemoteParticipants.Count.ToString();

            if (_createdAtText != null)
            {
                DateTime createdAtDateTime = new DateTime(1970, 1, 1);
                createdAtDateTime = createdAtDateTime.AddMilliseconds(_sbRoom.CreatedAt);
                _createdAtText.text = createdAtDateTime.ToLocalTime().ToString("M/d/yy HH:mm:ss");
            }
        }

        private void OnClickEnterRoomButton()
        {
            if (_sbRoom != null)
            {
                SbRoomEnterParams roomEnterParams = new SbRoomEnterParams { AudioEnabled = true };
                _sbRoom.Enter(roomEnterParams, EnterRoomResultHandler);
            }
            else
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup("RoomListScrollViewContent::OnClickEnterRoomButton _sbRoom is null");
            }
        }

        private void EnterRoomResultHandler(SbError error)
        {
            if (null == error)
            {
                RoomState roomState = SampleGroupCallMain.Instance.OpenState(StateType.Room) as RoomState;
                if (roomState != null)
                    roomState.EnterRoom(_sbRoom);
            }
            else
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup($"EnterRoomResultHandler ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
            }
        }
    }
}