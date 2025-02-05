using System;
using UnityEngine;

namespace Utils.Animation
{
    public enum AnimatorStateEventType
    {
        onEnter,
        onExit,
    }

    public class AnimatorStateChangeInvoker : StateMachineBehaviour
    {
        public event Action<AnimatorStateInfo, AnimatorStateEventType> OnState;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnState?.Invoke(stateInfo, AnimatorStateEventType.onEnter);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnState?.Invoke(stateInfo, AnimatorStateEventType.onExit);
        }
    }
}
