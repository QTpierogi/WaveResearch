using UnityEngine;
using UnityEngine.Events;

namespace WaveProject.Interaction
{
    internal class Toggle : Interactable
    {
        [SerializeField] private UnityEvent<bool> _toggled;
        
        public override bool OneClickInteracting => true;

        public UnityEvent<bool> Toggled => _toggled;

        private bool _turnOn;

        public void SetDefaultToggledState(bool value)
        {
            _turnOn = value;
        }

        public override void CustomUpdate(Vector2 delta)
        {
            _turnOn = !_turnOn;
            Toggled?.Invoke(_turnOn);
            FinishChanging();
        }
    }
}