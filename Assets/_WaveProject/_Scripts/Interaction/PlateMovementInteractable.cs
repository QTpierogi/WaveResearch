using UnityEngine;

namespace WaveProject.Interaction
{
    public class PlateMovementInteractable : MoveBetweenPointsInteractable
    {
        [SerializeField] private Transform _correctPoint;
        [SerializeField] private float _correctBuffer = 1f;
        
        private Color _currentColor;

        public Transform StartPoint => _leftPoint;

        public override void CustomUpdate(Vector2 delta)
        {
            UpdateDeltaDistance(delta);
            
            var time = TotalDeltaDistance * Sensitivity;
            SetPosition(time);

            SetColor();

            if (!Input.GetMouseButtonUp(0)) return;
            if (IsCorrectPlace())
            {
                FinishChanging();
            }
        }

        private void SetColor()
        {
            var color = IsCorrectPlace() ? Color.green : Color.red;

            if (color == _currentColor)
                return;
            
            _currentColor = color;

            Outline.OutlineColor = _currentColor;
        }

        private bool IsCorrectPlace() => Mathf.Abs(_correctPoint.position.z - transform.position.z) <= _correctBuffer;
    }
}