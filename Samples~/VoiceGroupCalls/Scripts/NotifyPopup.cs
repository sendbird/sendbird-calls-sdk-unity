// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class NotifyPopup : MonoBehaviour
    {
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Button _collisionButton = null;
        [SerializeField] private Text _notifyText = null;

        private void Awake()
        {
            if (_closeButton == null) Debug.LogError("You need to set CloseButton in NotifyPopup");
            if (_notifyText == null) Debug.LogError("You need to set NotifyText in NotifyPopup");

            _closeButton.onClick.AddListener(OnClickCloseButton);

            if (_collisionButton != null) _collisionButton.onClick.AddListener(OnClickCloseButton);
        }

        public void Open(string notifyMessage)
        {
            gameObject.SetActive(true);
            if (_notifyText != null)
            {
                _notifyText.text = notifyMessage;
            }
        }

        private void OnClickCloseButton()
        {
            gameObject.SetActive(false);
        }
    }
}