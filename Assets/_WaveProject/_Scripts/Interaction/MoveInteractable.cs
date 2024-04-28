using UnityEngine;
using WaveProject.Utility;

namespace WaveProject.Interaction
{
    public class MoveInteractable : DirectionInteractable
    {
        [SerializeField] private Transform _closestPoint;
        [SerializeField] private Transform _farthestPoint;

        public void SetDefaultValue()
        {
            var start = _farthestPoint.position;
            var end = _closestPoint.position;
            
            var currentValue = Utils.InverseLerp(start, end, transform.position);
            
            var valuePercentage = currentValue;
            TotalDeltaDistance = valuePercentage / Sensitivity;
        }

        public override void CustomUpdate(Vector2 delta)
        {
            UpdateDeltaDistance(delta);

            var start = _farthestPoint.position;
            var end = _closestPoint.position;

            transform.position = Vector3.Lerp(start, end, TotalDeltaDistance * Sensitivity);
            
            if (Input.GetMouseButtonDown(0))
            {
                FinishChanging();
            }
        }
    }
}