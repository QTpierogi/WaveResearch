using System;
using UnityEngine;
using WaveProject.Interaction;

namespace WaveProject.UserInput
{
    public class Interactable : MonoBehaviour, IInputSubscriber, ISelectable
    {
        [field: SerializeField] public Outline Outline { get; private set; }
        [SerializeField] private float _angleRange = 70;
        [SerializeField] private float _sensitivity = 2;

        private float _totalDeltaDistance;
        
        public event Action ForceUnsubscribe;

        private void Start() => Deselect();

        private void OnValidate()
        {
            Outline ??= GetComponent<Outline>();
        }

        public void Enable()
        {
        }

        public void Disable()
        {
        }

        public void Select()
        {
            Outline.enabled = true;
        }

        public void Deselect()
        {
            Outline.enabled = false;
        }

        public void CustomUpdate(Vector2 delta)
        {
            _totalDeltaDistance += delta.x;
            _totalDeltaDistance = Mathf.Clamp(_totalDeltaDistance, 0, 1 / _sensitivity);
            
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(-_angleRange, 0, 0),
                Quaternion.Euler(_angleRange, 0, 0), _totalDeltaDistance * _sensitivity);

            if (Input.GetMouseButtonDown(0))
            {
                ForceUnsubscribe?.Invoke();
            }
        }
    }
}