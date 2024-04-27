using System;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.UserInput;

namespace WaveProject.Interaction
{
    [RequireComponent(typeof(Outline), typeof(Collider))]
    public abstract class Interactable : MonoBehaviour, IInputSubscriber, ISelectable
    {
        [field: SerializeField] public Outline Outline { get; private set; }

        [field: SerializeField] public InputAxis InputAxis { get; private set;}
        [field: SerializeField] public Vector3 ExitAxis { get; private set;} = Vector3.right;

        protected float Sensitivity;
        protected float TotalDeltaDistance;
        protected float MaxTotalDeltaDistance => 1 / Sensitivity;
        
        public event Action ChangingFinished;

        protected void OnValidate()
        {
            Outline ??= GetComponent<Outline>();
        }

        public void Init()
        {
            LoadData();
            Deselect();
        }

        private void LoadData() => Sensitivity = InteractionSettings.Data.Sensitivity;
        
        public Transform Transform => transform;
        public virtual bool OneClickInteracting => false;

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
        
        protected virtual void UpdateDeltaDistance(Vector2 delta)
        {
            TotalDeltaDistance += InputAxis == InputAxis.Horizontal ? delta.x : delta.y;
            TotalDeltaDistance = Mathf.Clamp(TotalDeltaDistance, 0, MaxTotalDeltaDistance);
        }

        protected void FinishChanging()
        {
            ChangingFinished?.Invoke();
        }
    }
}