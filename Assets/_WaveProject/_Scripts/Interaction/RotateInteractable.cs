using UnityEngine;

namespace WaveProject.Interaction
{
    internal class RotateInteractable : Interactable
    {
        [SerializeField] private float _angleRange = 70;

        public override void CustomUpdate(Vector2 delta)
        {
            UpdateDeltaDistance(delta);

            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(-_angleRange * _exitAxis.x, -_angleRange * _exitAxis.y,
                    -_angleRange * _exitAxis.z),
                Quaternion.Euler(_angleRange * _exitAxis.x, _angleRange * _exitAxis.y, _angleRange * _exitAxis.z),
                TotalDeltaDistance * Sensitivity);

            if (Input.GetMouseButtonDown(0))
            {
                FinishChanging();
            }
        }
        
    }
}