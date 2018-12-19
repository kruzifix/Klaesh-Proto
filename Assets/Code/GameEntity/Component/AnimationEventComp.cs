using System;
using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public class AnimationEventComp : MonoBehaviour
    {
        public event EventHandler<AnimationEvent> AnimationEvent;

        public void OnAnimationEvent(AnimationEvent data)
        {
            AnimationEvent?.Invoke(this, data);
        }
    }
}
