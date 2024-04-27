using UnityEngine;
using WaveProject.Utility;

namespace WaveProject.Interaction
{
    internal class RotateInteractable : Interactable
    {
        private float _minValue;
        private float _maxValue;
        
        [field: SerializeField] public float AngleRange { get; protected set; } = 90;

        public void SetDefaultValue(float currentValue, float minValue, float maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            
            Transform.rotation = Utils.GetRotationInRange(currentValue, minValue, maxValue,
                    -AngleRange, AngleRange, ExitAxis);

            var value = GetValue();
            TotalDeltaDistance = value / _maxValue / Sensitivity;
        }

        public float GetValue()
        {
            return Utils.GetValueByRotationInRange(Transform.rotation, -AngleRange,
                AngleRange, _minValue, _maxValue, ExitAxis);
        }

        public override void CustomUpdate(Vector2 delta)
        {
            UpdateDeltaDistance(delta);
            
            Transform.rotation = Utils.GetRotationInRange(TotalDeltaDistance * Sensitivity, 0, 1,
                -AngleRange, AngleRange, ExitAxis);

            if (Input.GetMouseButtonDown(0))
            {
                FinishChanging();
            }
        }
    }
}