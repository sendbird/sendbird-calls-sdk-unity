// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class CreateRoomPopup : MonoBehaviour
    {
        [SerializeField] private Button _createButton = null;
        [SerializeField] private Button _cancelButton = null;
        [SerializeField] private Button _collisionButton = null;
        [SerializeField] private InputField _customKeyInputField = null;
        [SerializeField] private InputField _customValueInputField = null;

        private Action _successCreateRoomDelegate = null;

        private void Awake()
        {
            if (_createButton == null) Debug.LogError("You need to set CreateButton in CreateRoomPopup");
            if (_cancelButton == null) Debug.LogError("You need to set CancelButton in CreateRoomPopup");

            _createButton.onClick.AddListener(OnClickCreateButton);
            _cancelButton.onClick.AddListener(OnClickCancelButton);

            if (_collisionButton != null) _collisionButton.onClick.AddListener(OnClickCancelButton);
        }

        public void Open(Action successCreateRoomDelegate)
        {
            gameObject.SetActive(true);
            _successCreateRoomDelegate = successCreateRoomDelegate;
        }

        private void OnClickCreateButton()
        {
            gameObject.SetActive(false);
            SbRoomParams roomParams = new SbRoomParams();
            if (_customKeyInputField != null && string.IsNullOrEmpty(_customKeyInputField.text) && _customValueInputField != null && string.IsNullOrEmpty(_customValueInputField.text))
            {
                roomParams.CustomItems.Add(_customKeyInputField.text, _customValueInputField.text);
            }

            SampleGroupCallMain.Instance.SetBlockingGameObjectActive(true);
            SendbirdCall.CreateRoom(roomParams, CreateRoomResultHandler);
        }

        private void CreateRoomResultHandler(SbRoom room, SbError error)
        {
            SampleGroupCallMain.Instance.SetBlockingGameObjectActive(false);

            if (error == null)
            {
                _successCreateRoomDelegate?.Invoke();
            }
            else
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup($"CreateRoomResultHandler ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
            }

            gameObject.SetActive(false);
        }

        private void OnClickCancelButton()
        {
            gameObject.SetActive(false);
        }
    }
}