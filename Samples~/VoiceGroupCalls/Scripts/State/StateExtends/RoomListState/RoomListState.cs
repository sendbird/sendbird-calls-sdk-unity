// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class RoomListState : StateAbstract
    {
        [SerializeField] private RoomListScrollView _roomListScrollView = null;
        [SerializeField] private Button _createRoomButton = null;
        [SerializeField] private CreateRoomPopup _createRoomPopup = null;

        private void Awake()
        {
            if (_roomListScrollView == null) Debug.LogError("You need to set RoomListScrollView in RoomListState");
            if (_createRoomPopup == null) Debug.LogError("You need to set CreateRoomPopup in RoomListState");

            if (_createRoomButton == null) Debug.LogError("You need to set CreateRoomButton in RoomListState");
            else _createRoomButton.onClick.AddListener(OnClickCreateButton);
        }

        public override StateType GetStateType()
        {
            return StateType.RoomList;
        }

        public override void OnOpenState()
        {
            _roomListScrollView.QueryRoomList();
            _createRoomPopup.gameObject.SetActive(false);
        }

        private void OnClickCreateButton()
        {
            _createRoomPopup.Open(_roomListScrollView.QueryRoomList);
        }
    }
}