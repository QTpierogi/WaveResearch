using System;
using UnityEngine;

namespace WaveProject.UserInput
{
    public interface IInputSubscriber
    {
        public event Action ForceUnsubscribed;
        Transform Transform { get; }
        
        void Enable();
        void Disable();
        void CustomUpdate(Vector2 delta);
    }
}