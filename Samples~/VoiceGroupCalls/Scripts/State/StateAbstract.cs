// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

using UnityEngine;

namespace Sendbird.Calls.Sample
{
    public abstract class StateAbstract : MonoBehaviour
    {
        public abstract StateType GetStateType();

        public virtual void OnOpenState() { }
    }
}