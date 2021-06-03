using System;
using System.Collections.Generic;
using UnityEngine;

namespace HM {
    [RequireComponent(typeof(Animator))]
    public class AnimatorEventsHandler : MonoBehaviour {
        public event Action<string> OnEventFired;

        public void EventHandler(string parameter) {
            OnEventFired?.Invoke(parameter);
        }
    }
}