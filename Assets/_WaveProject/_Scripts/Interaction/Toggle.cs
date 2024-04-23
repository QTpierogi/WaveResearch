using UnityEngine;
using UnityEngine.Events;

namespace WaveProject.Interaction
{
    internal class Toggle : Interactable
    {
        [SerializeField] private UnityEvent<bool> _toggled;
        
        public override bool OneClickInteracting => true;

        private bool _turnOn;

        public override void CustomUpdate(Vector2 delta)
        {
            _turnOn = !_turnOn;
            _toggled?.Invoke(_turnOn);
            FinishChanging();
        }
    }
}