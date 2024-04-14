using System;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.UserInput;

namespace WaveProject.Interaction
{
    [RequireComponent(typeof(Outline), typeof(Collider))]
    public class Interactable : MonoBehaviour, IInputSubscriber, ISelectable
    {
        [field: SerializeField] public Outline Outline { get; private set; }
        
        [SerializeField] protected InputAxis _inputAxis;
        [SerializeField] protected Vector3 _exitAxis = Vector3.right;
        
        protected float Sensitivity;
        protected float TotalDeltaDistance;
        
        public event Action ChangingFinished;
        
        protected virtual void Start()
        {
            // _totalDeltaDistance = Utils.GetValueByRotationInRange(transform.rotation, -_angleRange,
            //     _angleRange, 0, _MAX_FREQUENCY, Vector3.right) / _MAX_FREQUENCY / _sensitivity;

            LoadData();
            Deselect();
        }

        private void LoadData() => Sensitivity = InteractionSettings.Data.Sensitivity;

        protected void OnValidate()
        {
            Outline ??= GetComponent<Outline>();
        }

        public Transform Transform => transform;

        public virtual void Enable() { }

        public virtual void Disable() { }

        public virtual void CustomUpdate(Vector2 delta) { }

        public virtual void Select()
        {
            Outline.enabled = true;
        }

        public virtual void Deselect()
        {
            Outline.enabled = false;
        }
        
        protected void UpdateDeltaDistance(Vector2 delta)
        {
            TotalDeltaDistance += _inputAxis == InputAxis.Horizontal ? delta.x : delta.y;
            TotalDeltaDistance = Mathf.Clamp(TotalDeltaDistance, 0, 1 / Sensitivity);
        }

        protected void FinishChanging()
        {
            ChangingFinished?.Invoke();
        }
    }
}