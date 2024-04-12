using System;
using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Utility;

namespace WaveProject.UserInput
{
    [RequireComponent(typeof(Outline), typeof(Collider))]
    public class RotateInteractable : MonoBehaviour, IInputSubscriber, ISelectable
    {
        [field: SerializeField] public Outline Outline { get; private set; }
        
        [SerializeField] protected InputAxis _inputAxis;
        [SerializeField] protected Vector3 _rotateAxis = Vector3.right;
        
        [SerializeField] private float _angleRange = 70;
        [SerializeField] protected float _sensitivity = 0.005f;

        protected float TotalDeltaDistance;

        public event Action ForceUnsubscribed;

        protected virtual void Start()
        {
            // _totalDeltaDistance = Utils.GetValueByRotationInRange(transform.rotation, -_angleRange,
            //     _angleRange, 0, _MAX_FREQUENCY, Vector3.right) / _MAX_FREQUENCY / _sensitivity;
            
            Deselect();
        }

        protected void OnValidate()
        {
            Outline ??= GetComponent<Outline>();
        }

        public Transform Transform => transform;

        public virtual void Enable()
        {
        }

        public virtual void Disable()
        {
        }

        public virtual void Select()
        {
            Outline.enabled = true;
        }

        public virtual void Deselect()
        {
            Outline.enabled = false;
        }

        public virtual void CustomUpdate(Vector2 delta)
        {
            UpdateDeltaDistance(delta);

            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(-_angleRange * _rotateAxis.x, -_angleRange * _rotateAxis.y,
                    -_angleRange * _rotateAxis.z),
                Quaternion.Euler(_angleRange * _rotateAxis.x, _angleRange * _rotateAxis.y, _angleRange * _rotateAxis.z),
                TotalDeltaDistance * _sensitivity);

            if (Input.GetMouseButtonDown(0))
            {
                ForceUnsubscribe();
            }
        }
        
        protected void UpdateDeltaDistance(Vector2 delta)
        {
            TotalDeltaDistance += _inputAxis == InputAxis.Horizontal ? delta.x : delta.y;
            TotalDeltaDistance = Mathf.Clamp(TotalDeltaDistance, 0, 1 / _sensitivity);
        }

        protected void ForceUnsubscribe()
        {
            ForceUnsubscribed?.Invoke();
        }
    }
}