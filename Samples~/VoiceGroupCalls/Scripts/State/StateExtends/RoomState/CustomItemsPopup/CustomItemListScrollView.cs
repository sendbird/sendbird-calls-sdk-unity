// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace Sendbird.Calls.Sample
{
    public class CustomItemListScrollView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private CustomItemListScrollViewContent _customItemListScrollViewContentPrefab = null;
        [SerializeField] private Button _addButton = null;
        [SerializeField] private GameObject _emptyNotifyItemGameObject = null;
        [SerializeField] private Text _countText = null;
        private readonly List<CustomItemListScrollViewContent> _activeScrollContents = new List<CustomItemListScrollViewContent>();

        private readonly Queue<CustomItemListScrollViewContent> _scrollContentsPool = new Queue<CustomItemListScrollViewContent>();
        private Coroutine _refreshScrollCoroutine = null;

        public Dictionary<string, string> EditedCustomItems { get; } = new Dictionary<string, string>();
        public Action<bool> OnChangeEditedCustomItems { get; set; } = null;

        private void Awake()
        {
            if (_scrollRect == null) Debug.LogError("You need to set ScrollRect in CustomItemListScrollView");
            if (_customItemListScrollViewContentPrefab == null) Debug.LogError("You need to set CustomItemListScrollViewContentPrefab in CustomItemListScrollView");

            if (_addButton == null) Debug.LogError("You need to set AddButton in CustomItemListScrollView");
            else _addButton.onClick.AddListener(OnClickAddButton);
        }

        public void ResetFromRoomCustomItems(ReadOnlyDictionary<string, string> roomCustomItems)
        {
            StopRefreshAndRestoreAllScrollViewContents();

            EditedCustomItems.Clear();
            foreach (KeyValuePair<string, string> keyValuePair in roomCustomItems) EditedCustomItems.Add(keyValuePair.Key, keyValuePair.Value);

            if (_emptyNotifyItemGameObject != null)
                _emptyNotifyItemGameObject.SetActive(false);

            _refreshScrollCoroutine = StartCoroutine(RefreshScrollContentsCoroutine());
        }

        private IEnumerator RefreshScrollContentsCoroutine()
        {
            if (_scrollRect == null || _scrollRect.content == null)
                yield break;

            foreach (KeyValuePair<string, string> keyValuePair in EditedCustomItems)
            {
                AddScrollContent(keyValuePair.Key, keyValuePair.Value);
                yield return null;
            }

            RefreshCountAndNotifyObjects();

            yield return null;
            _refreshScrollCoroutine = null;
        }

        private void AddScrollContent(string customItemKey, string customItemValue)
        {
            CustomItemListScrollViewContent roomListScrollViewContent = GetScrollViewContentFromPool();
            roomListScrollViewContent.SetKeyString(customItemKey);
            roomListScrollViewContent.SetValueString(customItemValue);
            roomListScrollViewContent.OnChangeKeyOrValueDelegate = OnChangeCustomItemKeyOrValue;
            roomListScrollViewContent.OnDeleteDelegate = OnDeleteCustomItem;
            roomListScrollViewContent.transform.SetParent(_scrollRect.content);
            _activeScrollContents.Add(roomListScrollViewContent);
            _addButton.transform.SetAsLastSibling();
        }

        private CustomItemListScrollViewContent GetScrollViewContentFromPool()
        {
            CustomItemListScrollViewContent scrollViewContent = null;
            if (0 < _scrollContentsPool.Count)
            {
                scrollViewContent = _scrollContentsPool.Dequeue();
            }
            else
            {
                scrollViewContent = Instantiate(_customItemListScrollViewContentPrefab, transform);
            }

            scrollViewContent.gameObject.SetActive(true);
            return scrollViewContent;
        }

        private void RestoreScrollViewContentToPool(CustomItemListScrollViewContent scrollViewContent)
        {
            if (scrollViewContent == null)
                return;

            scrollViewContent.OnChangeKeyOrValueDelegate = null;
            scrollViewContent.OnDeleteDelegate = null;
            scrollViewContent.transform.SetParent(transform);
            scrollViewContent.gameObject.SetActive(false);
            _scrollContentsPool.Enqueue(scrollViewContent);
        }

        private void StopRefreshAndRestoreAllScrollViewContents()
        {
            if (_refreshScrollCoroutine != null)
            {
                StopCoroutine(_refreshScrollCoroutine);
                _refreshScrollCoroutine = null;
            }

            foreach (CustomItemListScrollViewContent scrollItem in _activeScrollContents) RestoreScrollViewContentToPool(scrollItem);

            _activeScrollContents.Clear();
        }

        private void RefreshCountAndNotifyObjects()
        {
            if (_countText != null)
            {
                const int CUSTOM_ITEM_LIMIT = 10;
                _countText.text = $"({_activeScrollContents.Count}/{CUSTOM_ITEM_LIMIT})";
            }

            if (_emptyNotifyItemGameObject != null)
            {
                bool active = !(0 < _activeScrollContents.Count);
                _emptyNotifyItemGameObject.gameObject.SetActive(active);
            }
        }

        private void OnChangeCustomItemKeyOrValue()
        {
            ResetEditedCustomItemsFromActiveScrollContents();
        }

        private void OnDeleteCustomItem(CustomItemListScrollViewContent deletedCustomItemListScrollViewContent)
        {
            _activeScrollContents.Remove(deletedCustomItemListScrollViewContent);
            RestoreScrollViewContentToPool(deletedCustomItemListScrollViewContent);
            ResetEditedCustomItemsFromActiveScrollContents();
        }

        private void ResetEditedCustomItemsFromActiveScrollContents()
        {
            EditedCustomItems.Clear();
            bool hasInvalidKey = false;
            foreach (CustomItemListScrollViewContent customItemListScrollViewContent in _activeScrollContents)
            {
                string key = customItemListScrollViewContent.GetKeyString();
                if (string.IsNullOrEmpty(key) == false)
                {
                    EditedCustomItems.Add(key, customItemListScrollViewContent.GetValueString());
                }
                else
                {
                    hasInvalidKey = true;
                }
            }

            RefreshCountAndNotifyObjects();

            OnChangeEditedCustomItems?.Invoke(hasInvalidKey);
        }

        private void OnClickAddButton()
        {
            AddScrollContent(string.Empty, string.Empty);
            ResetEditedCustomItemsFromActiveScrollContents();
        }
    }
}