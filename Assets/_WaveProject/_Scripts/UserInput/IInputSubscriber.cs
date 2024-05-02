using System;
using UnityEngine;

namespace WaveProject.UserInput
{
    public interface IInputSubscriber
    {
        public event Action ChangingFinished;
        bool OneClickInteracting { get; }

        const string LAYER = "Interactable";
        static LayerMask LayerMask => LayerMask.GetMask(LAYER);

        void Enable();
        void Disable();
        void CustomUpdate(Vector2 delta);
    }
}