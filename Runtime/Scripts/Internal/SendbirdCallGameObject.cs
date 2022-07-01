// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using System;
using System.Collections;
using Newtonsoft.Json.Utilities;
using UnityEngine;

namespace Sendbird.Calls
{
    internal class SendbirdCallGameObject : MonoBehaviourSingletonAbstract<SendbirdCallGameObject>
    {
        private SendbirdCallGameObject() { }

        private void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            
            //Bypass code stripping when build
            AotHelper.EnsureList<LogItemCommandObject>();
            AotHelper.EnsureList<RoomCommandObject>();
            AotHelper.EnsureList<ParticipantCommandObject>();
            AotHelper.EnsureList<UserCommandObject>();
        }

        internal void CallOnNextFrame(Action action)
        {
            StartCoroutine(CallOnNextFrameCoroutine(action));
        }

        private IEnumerator CallOnNextFrameCoroutine(Action action)
        {
            if (action == null)
                yield break;
            
            yield return null;
            action.Invoke();
        }

        private void OnApplicationQuit()
        { 
            SendbirdCallSdk.Instance.OnApplicationQuit();
        }
    }
}