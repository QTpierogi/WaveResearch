using UnityEngine;

namespace WaveProject.UserInput
{
    public class MoveInteractable : RotateInteractable
    {
        [SerializeField] private Transform _closestPoint;
        [SerializeField] private Transform _farthestPoint;

        public override void CustomUpdate(Vector2 delta)
        {
            UpdateDeltaDistance(delta);

            var start = _farthestPoint.position;
            var end = _closestPoint.position;

            transform.position = Vector3.Lerp(start, end, TotalDeltaDistance * _sensitivity);
            
            if (Input.GetMouseButtonDown(0))
            {
                ForceUnsubscribe();
            }
        }
    }
}