// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;

namespace Sendbird.Calls
{
    internal abstract class MonoBehaviourSingletonAbstract<TClassType> : MonoBehaviour where TClassType : MonoBehaviour
    {
        private static TClassType _internalInstance = null;

        internal static TClassType Instance
        {
            get
            {
                if (_internalInstance == null)
                    CreateInstance();

                return _internalInstance;
            }
        }

        private static void CreateInstance()
        {
            if (_internalInstance != null)
                return;

            _internalInstance = FindObjectOfType<TClassType>();
            if (_internalInstance != null)
                return;

            GameObject gameObject = new GameObject(typeof(TClassType).Name);
            DontDestroyOnLoad(gameObject);
            _internalInstance = gameObject.AddComponent<TClassType>();
        }
    }
}