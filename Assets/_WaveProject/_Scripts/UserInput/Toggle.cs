using UnityEngine;
using UnityEngine.Events;

namespace WaveProject.UserInput
{
    class Toggle : Interactable
    {
        [SerializeField] private UnityEvent<bool> _toggleSwithched;
        
        private bool _turnOn;

        public override void CustomUpdate(Vector2 delta)
        {
            _turnOn = !_turnOn;
            _toggleSwithched?.Invoke(_turnOn);
            ForceUnsubscribe();
        }
    }
}