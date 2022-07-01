// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class AuthenticateState : StateAbstract
    {
        [SerializeField] private Button _authenticateButton = null;
        [SerializeField] private InputField _appIdInputField = null;
        [SerializeField] private InputField _userIdInputField = null;
        [SerializeField] private InputField _accessTokenInputField = null;

        private void Awake()
        {
            if (_appIdInputField != null) _appIdInputField.onValueChanged.AddListener(OnChangeAppId);
            else Debug.LogError("You need to set AppId InputField");

            if (_userIdInputField != null) _userIdInputField.onValueChanged.AddListener(OnChangeUserId);
            else Debug.LogError("You need to set UserId InputField");

            if (_authenticateButton != null) _authenticateButton.onClick.AddListener(OnClickAuthenticateButton);
            else Debug.LogError("You need to set Authenticate Button");
        }

        private void Start()
        {
            if (_appIdInputField != null)
            {
                _appIdInputField.text = SampleGroupCallMain.Instance.AppId;
            }
        }

        public override StateType GetStateType()
        {
            return StateType.Authenticate;
        }

        public override void OnOpenState()
        {
            CheckAuthenticateButtonInteractable();
        }

        private void OnChangeAppId(string text)
        {
            CheckAuthenticateButtonInteractable();
        }

        private void OnChangeUserId(string text)
        {
            CheckAuthenticateButtonInteractable();
        }

        private void OnClickAuthenticateButton()
        {
            string appId = _appIdInputField.text;
            if (string.IsNullOrEmpty(appId))
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup("You must write AppId");
                return;
            }

            if (SendbirdCall.Init(appId) == false)
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup("Failed SendbirdCall init");
                return;
            }

            string userId = _userIdInputField.text;
            if (string.IsNullOrEmpty(userId))
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup("You must write UserId");
                return;
            }

            string accessTokenIfExist = string.Empty;
            if (_accessTokenInputField != null)
                accessTokenIfExist = _accessTokenInputField.text;

            SampleGroupCallMain.Instance.SetBlockingGameObjectActive(true);

            _authenticateButton.interactable = false;
            SbAuthenticateParams sbAuthenticateParams = new SbAuthenticateParams(userId) { AccessToken = accessTokenIfExist };

            SendbirdCall.Authenticate(sbAuthenticateParams, AuthenticateResultHandler);
        }

        private void AuthenticateResultHandler(SbUser user, SbError error)
        {
            SampleGroupCallMain.Instance.SetBlockingGameObjectActive(false);

            if (error == null)
            {
                SampleGroupCallMain.Instance.OpenState(StateType.RoomList);
            }
            else
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup($"ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
            }

            CheckAuthenticateButtonInteractable();
        }

        private void CheckAuthenticateButtonInteractable()
        {
            _authenticateButton.interactable = !string.IsNullOrEmpty(_userIdInputField.text);
        }
    }
}