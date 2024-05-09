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
        private Vector3 _defaultScale;

        public void SetDefaultToggledState(bool value)
        {
            _turnOn = value;
            _defaultScale = transform.localScale;
        }

        public override void CustomUpdate(Vector2 delta)
        {
            _turnOn = !_turnOn;

            var scaleFactor = new Vector3(1, _turnOn ? -1 : 1, 1);
            var newScale = _defaultScale;
            newScale.Scale(scaleFactor);
            
            transform.localScale = newScale;
            
            Toggled?.Invoke(_turnOn);
            FinishChanging();
        }
    }
}