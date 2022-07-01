// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;

namespace Sendbird.Calls.Sample
{
    public class SampleGroupCallMain : MonoBehaviour
    {
        [SerializeField] private string _appId = "452AAA78-B75E-4E10-B06F-F5D85D1651EC";
        [SerializeField] private StateAbstract[] _states = null;
        [SerializeField] private GameObject _blockingGameObject = null;
        [SerializeField] private NotifyPopup _notifyPopup = null;
        public string AppId => _appId;

        public static SampleGroupCallMain Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            SetBlockingGameObjectActive(false);

            OpenState(StateType.Authenticate);
        }

        public StateAbstract OpenState(StateType stateType)
        {
            if (_states == null)
                return null;

            StateAbstract openedState = null;
            foreach (StateAbstract state in _states)
            {
                if (state == null)
                    continue;

                bool canOpen = state.GetStateType() == stateType;
                state.gameObject.SetActive(canOpen);

                if (canOpen)
                {
                    state.OnOpenState();
                    openedState = state;
                }
            }

            return openedState;
        }

        public void SetBlockingGameObjectActive(bool show)
        {
            if (_blockingGameObject != null)
            {
                _blockingGameObject.gameObject.SetActive(show);
            }
        }

        public void OpenNotifyPopup(string notifyMessage)
        {
            if (_notifyPopup != null)
            {
                _notifyPopup.Open(notifyMessage);
            }
        }
    }
}