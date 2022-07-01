// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class CustomItemListScrollViewContent : MonoBehaviour
    {
        [SerializeField] private Button _deleteButton = null;
        [SerializeField] private InputField _keyInputField = null;
        [SerializeField] private InputField _valueInputField = null;
        [SerializeField] private Text _invalidKeyWarningText = null;

        public Action OnChangeKeyOrValueDelegate { get; set; } = null;
        public Action<CustomItemListScrollViewContent> OnDeleteDelegate { get; set; } = null;

        private void Awake()
        {
            if (_deleteButton == null) Debug.LogError("You need to set DeleteButton in CustomItemListScrollViewContent");
            else _deleteButton.onClick.AddListener(OnClickDeleteButton);

            if (_keyInputField != null) _keyInputField.onValueChanged.AddListener(OnKeyOrValueInputChange);
            if (_valueInputField != null) _valueInputField.onValueChanged.AddListener(OnKeyOrValueInputChange);
        }

        public void SetKeyString(string key)
        {
            if (_keyInputField != null)
            {
                _keyInputField.text = key;
                CheckInvalidKeyAndShowWarningText();
            }
        }

        public void SetValueString(string value)
        {
            if (_valueInputField != null)
            {
                _valueInputField.text = value;
            }
        }

        public string GetKeyString()
        {
            return _keyInputField != null ? _keyInputField.text : string.Empty;
        }

        public string GetValueString()
        {
            return _valueInputField != null ? _valueInputField.text : string.Empty;
        }

        private void OnKeyOrValueInputChange(string value)
        {
            CheckInvalidKeyAndShowWarningText();
            OnChangeKeyOrValueDelegate?.Invoke();
        }

        private void OnClickDeleteButton()
        {
            OnDeleteDelegate?.Invoke(this);
        }

        private void CheckInvalidKeyAndShowWarningText()
        {
            if (_invalidKeyWarningText != null)
            {
                string key = _keyInputField != null ? _keyInputField.text : string.Empty;
                _invalidKeyWarningText.gameObject.SetActive(string.IsNullOrEmpty(key));
            }
        }
    }
}