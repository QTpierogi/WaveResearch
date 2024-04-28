using UnityEngine;
using WaveProject.UserInput;

namespace WaveProject.Interaction
{
    class InfiniteRotateInteractable : DirectionInteractable
    {
        public virtual void SetDefaultRotation()
        {
            Transform.rotation = Quaternion.identity;
        }

        protected override void UpdateDeltaDistance(Vector2 delta)
        {
            TotalDeltaDistance += InputAxis == InputAxis.Horizontal ? delta.x : delta.y;
        }

        public float GetRotation()
        {
            var rotationEulerAngles = transform.rotation.eulerAngles;
            rotationEulerAngles.Scale(ExitAxis);
            return rotationEulerAngles.magnitude;
        }

        public override void CustomUpdate(Vector2 delta)
        {
            UpdateDeltaDistance(delta);

            Transform.rotation = Quaternion.Euler(ExitAxis * TotalDeltaDistance);

            if (Input.GetMouseButtonDown(0))
            {
                FinishChanging();
            }
        }
    }
}