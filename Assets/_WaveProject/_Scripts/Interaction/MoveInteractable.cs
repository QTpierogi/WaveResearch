using UnityEngine;

namespace WaveProject.Interaction
{
    public class MoveInteractable : Interactable
    {
        [SerializeField] private Transform _closestPoint;
        [SerializeField] private Transform _farthestPoint;

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