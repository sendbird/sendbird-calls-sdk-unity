// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class CustomItemsPopup : MonoBehaviour
    {
        [SerializeField] private Button _saveButton = null;
        [SerializeField] private Button _cancelButton = null;
        [SerializeField] private CustomItemListScrollView _customItemListScrollView = null;
        [SerializeField] private Button _collisionButton = null;
        private ReadOnlyDictionary<string, string> _roomCustomItemsByKey = null;

        private SbRoom _sbRoom = null;

        private void Awake()
        {
            if (_saveButton == null) Debug.LogError("You need to set SaveButton in CustomItemsPopup");
            if (_cancelButton == null) Debug.LogError("You need to set CancelButton in CustomItemsPopup");
            if (_customItemListScrollView == null) Debug.LogError("You need to set CustomItemListScrollView in CustomItemsPopup");

            _saveButton.onClick.AddListener(OnClickSaveButton);
            _cancelButton.onClick.AddListener(OnClickCancelButton);
            _customItemListScrollView.OnChangeEditedCustomItems = OnChangeEditedCustomItems;

            if (_collisionButton != null) _collisionButton.onClick.AddListener(OnClickCancelButton);
        }

        public void Open(SbRoom room)
        {
            gameObject.SetActive(true);

            _sbRoom = room;
            FetchCustomItemsAsync();
        }

        public void Close()
        {
            _sbRoom = null;
            gameObject.SetActive(false);
        }

        private void OnClickSaveButton()
        {
            StartCoroutine(DeleteAndUpdateCustomItemsCoroutine());
        }

        private void OnClickCancelButton()
        {
            Close();
        }

        private void OnChangeEditedCustomItems(bool hasInvalidKey)
        {
            _saveButton.interactable = !hasInvalidKey;
        }

        private void FetchCustomItemsAsync()
        {
            if (_sbRoom == null)
                return;

            SampleGroupCallMain.Instance.SetBlockingGameObjectActive(true);

            void FetchCustomItemsResultHandler(ReadOnlyDictionary<string, string> roomCustomItems, SbError error)
            {
                SampleGroupCallMain.Instance.SetBlockingGameObjectActive(false);

                if (error == null)
                {
                    _roomCustomItemsByKey = new ReadOnlyDictionary<string, string>(roomCustomItems);
                    _customItemListScrollView.ResetFromRoomCustomItems(_roomCustomItemsByKey);
                }
                else
                {
                    Debug.LogError($"FetchCustomItemsResultHandler ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
                }
            }

            _sbRoom.FetchCustomItems(FetchCustomItemsResultHandler);
        }

        private IEnumerator DeleteAndUpdateCustomItemsCoroutine()
        {
            Dictionary<string, string> updateCustomItems = new Dictionary<string, string>(_customItemListScrollView.EditedCustomItems);
            List<string> deleteCustomItemKeys = new List<string>();

            foreach (KeyValuePair<string, string> customItemKeyValuePair in _roomCustomItemsByKey)
            {
                if (_customItemListScrollView.EditedCustomItems.TryGetValue(customItemKeyValuePair.Key, out string outEditedValue))
                {
                    if (customItemKeyValuePair.Value.Equals(outEditedValue)) updateCustomItems.Remove(customItemKeyValuePair.Key);
                }
                else
                {
                    updateCustomItems.Remove(customItemKeyValuePair.Key);
                    deleteCustomItemKeys.Add(customItemKeyValuePair.Key);
                }
            }

            yield return StartCoroutine(DeleteCustomItemsCoroutine(deleteCustomItemKeys));
            yield return StartCoroutine(UpdateCustomItemsCoroutine(updateCustomItems));
        }

        private IEnumerator UpdateCustomItemsCoroutine(Dictionary<string, string> updateCustomItems)
        {
            if (_sbRoom == null || updateCustomItems == null || updateCustomItems.Count <= 0)
                yield break;

            bool waitingForResult = true;

            void UpdateCustomItemsResultHandler(ReadOnlyDictionary<string, string> roomCustomItems, ReadOnlyCollection<string> inAffectedKeys, SbError error)
            {
                waitingForResult = false;
                if (error == null)
                {
                    _roomCustomItemsByKey = roomCustomItems;
                    _customItemListScrollView.ResetFromRoomCustomItems(_roomCustomItemsByKey);
                }
                else
                {
                    Debug.LogError($"UpdateCustomItemsResultHandler ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
                }
            }

            _sbRoom.UpdateCustomItems(updateCustomItems, UpdateCustomItemsResultHandler);

            while (waitingForResult)
                yield return null;
        }

        private IEnumerator DeleteCustomItemsCoroutine(List<string> deleteCustomItemKeys)
        {
            if (_sbRoom == null || deleteCustomItemKeys == null || deleteCustomItemKeys.Count <= 0)
                yield break;

            bool waitingForResult = true;

            void DeleteCustomItemsResultHandler(ReadOnlyDictionary<string, string> roomCustomItems, ReadOnlyCollection<string> affectedKeys, SbError error)
            {
                waitingForResult = false;
                if (error == null)
                {
                    _roomCustomItemsByKey = roomCustomItems;
                    _customItemListScrollView.ResetFromRoomCustomItems(_roomCustomItemsByKey);
                }
                else
                {
                    Debug.LogError($"DeleteCustomItemsAsync ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
                }
            }

            _sbRoom.DeleteCustomItems(deleteCustomItemKeys, DeleteCustomItemsResultHandler);

            while (waitingForResult)
                yield return null;
        }
    }
}