using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.Animation
{
    [Serializable]
    public class AnimatorStateMethodExecutor : MonoBehaviour
    {
        public Animator Animator;
        public List<AnimationMethod> MethodList = new List<AnimationMethod>();

        private AnimatorStateChangeInvoker stateInvoker;
        private Dictionary<string, int> stashedStatesHashe;

        private void Awake()
        {
            stateInvoker = Animator.GetBehaviour<AnimatorStateChangeInvoker>();

            stashedStatesHashe = new();
            foreach (var method in MethodList)
            {
                if (stashedStatesHashe.ContainsKey(method.AnimationStateName)) continue;
                var hash = Animator.StringToHash(method.AnimationStateName);
                stashedStatesHashe.Add(method.AnimationStateName, hash);
            }

            stateInvoker.OnState += OnState;
        }

        private void OnDestroy()
        {
            stateInvoker.OnState -= OnState;
        }

        private void OnState(AnimatorStateInfo stateInfo, AnimatorStateEventType stateEventType)
        {
            var targetMethods = MethodList
                .Where(x => 
                x.ExecuteTime == stateEventType &&
                stashedStatesHashe[x.AnimationStateName] == stateInfo.shortNameHash
                );

            foreach (var method in targetMethods)
                method.Execute();
        }
    }
}
