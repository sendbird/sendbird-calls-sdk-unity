// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable All

namespace Sendbird.Calls.Sample
{
    public class RoomListScrollView : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RoomListScrollViewContent _roomListScrollViewContentPrefab = null;
        [SerializeField] private Button _queryNextRoomsButton = null;
        [SerializeField] private GameObject _emptyNotifyGameObject = null;
        private readonly List<RoomListScrollViewContent> _activeScrollContents = new List<RoomListScrollViewContent>();
        private readonly List<SbRoom> _sbRooms = new List<SbRoom>();

        private readonly Queue<RoomListScrollViewContent> _scrollContentsPool = new Queue<RoomListScrollViewContent>();
        private int _lastRoomIndex = 0;
        private Coroutine _refreshScrollCoroutine = null;
        private SbRoomListQuery _sbRoomListQuery = null;

        private void Awake()
        {
            if (_scrollRect == null) Debug.LogError("You need to set ScrollRect in RoomListScrollView");
            if (_roomListScrollViewContentPrefab == null) Debug.LogError("You need to set RoomScrollViewContentPrefab in RoomListScrollView");
            if (_queryNextRoomsButton == null) Debug.LogError("You need to set QueryNextRoomsButton in RoomListScrollView");

            _queryNextRoomsButton.onClick.AddListener(OnClickQueryNextRoomsButton);
        }

        private void OnDisable()
        {
            _sbRoomListQuery = null;
        }

        public void QueryRoomList()
        {
            StopRefreshAndRestoreAllScrollViewContents();

            _sbRooms.Clear();
            _queryNextRoomsButton.interactable = false;
            _lastRoomIndex = 0;

            SampleGroupCallMain.Instance.SetBlockingGameObjectActive(true);

            SbRoomListQueryParams sbRoomListQueryParams = new SbRoomListQueryParams();
            _sbRoomListQuery = SendbirdCall.CreateRoomListQuery(sbRoomListQueryParams);
            _sbRoomListQuery.Next(RoomListQueryResultHandler);
        }

        private void RoomListQueryResultHandler(ReadOnlyCollection<SbRoom> rooms, SbError error)
        {
            SampleGroupCallMain.Instance.SetBlockingGameObjectActive(false);

            _queryNextRoomsButton.interactable = _sbRoomListQuery.HasNext;

            if (error != null)
            {
                SampleGroupCallMain.Instance.OpenNotifyPopup($"RoomListQueryResultHandler ErrorCode:{error.ErrorCode} ErrorMessage:{error.ErrorMessage}");
                return;
            }

            _sbRooms.AddRange(rooms);

            if (_emptyNotifyGameObject != null)
            {
                _emptyNotifyGameObject.gameObject.SetActive(_sbRooms.Count == 0);
            }

            RefreshScrollContents();
        }

        private void OnClickQueryNextRoomsButton()
        {
            if (_sbRoomListQuery != null && _sbRoomListQuery.HasNext)
            {
                _sbRoomListQuery.Next(RoomListQueryResultHandler);
            }
        }

        private void RefreshScrollContents()
        {
            if (_refreshScrollCoroutine != null)
            {
                StopCoroutine(_refreshScrollCoroutine);
                _refreshScrollCoroutine = null;
            }

            _refreshScrollCoroutine = StartCoroutine(RefreshScrollContentsCoroutine());
        }

        private IEnumerator RefreshScrollContentsCoroutine()
        {
            if (_scrollRect == null || _scrollRect.content == null)
                yield break;

            int startIndex = _lastRoomIndex;
            for (int index = startIndex; index < _sbRooms.Count; index++)
            {
                SbRoom sbRoom = _sbRooms[index];
                if (sbRoom == null)
                    continue;

                RoomListScrollViewContent roomListScrollViewContent = GetScrollViewContentFromPool();
                roomListScrollViewContent.ResetFromSbRoom(sbRoom);
                roomListScrollViewContent.transform.SetParent(this._scrollRect.content);
                this._activeScrollContents.Add(roomListScrollViewContent);

                _lastRoomIndex = index;
                yield return null;
            }

            yield return null;
            _refreshScrollCoroutine = null;
        }

        private RoomListScrollViewContent GetScrollViewContentFromPool()
        {
            RoomListScrollViewContent scrollViewContent = null;
            if (0 < _scrollContentsPool.Count)
            {
                scrollViewContent = _scrollContentsPool.Dequeue();
            }
            else
            {
                scrollViewContent = Instantiate(_roomListScrollViewContentPrefab, transform);
            }

            scrollViewContent.gameObject.SetActive(true);
            return scrollViewContent;
        }

        private void RestoreScrollViewContentToPool(RoomListScrollViewContent scrollViewContent)
        {
            if (scrollViewContent == null)
                return;

            scrollViewContent.transform.SetParent(transform);
            scrollViewContent.gameObject.SetActive(false);
            _scrollContentsPool.Enqueue(scrollViewContent);
        }

        private void StopRefreshAndRestoreAllScrollViewContents()
        {
            if (_refreshScrollCoroutine != null)
            {
                StopCoroutine(this._refreshScrollCoroutine);
                _refreshScrollCoroutine = null;
            }

            foreach (RoomListScrollViewContent scrollItem in this._activeScrollContents)
            {
                RestoreScrollViewContentToPool(scrollItem);
            }

            _activeScrollContents.Clear();
            _lastRoomIndex = 0;
        }
    }
}