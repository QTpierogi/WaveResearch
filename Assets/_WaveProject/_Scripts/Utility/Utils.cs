using UnityEngine;

namespace WaveProject.Utility
{
    public static class Utils
    {
        public static float Remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }

        public static Quaternion GetRotationInRange(float currentValue, float in1, float in2, float minAngleRange,
            float maxAngleRange, Vector3 rotationAxis)
        {
            var angle = Remap(currentValue, in1, in2, minAngleRange, maxAngleRange);
            var clampedRotation = Mathf.Clamp(angle, minAngleRange, maxAngleRange);
            return Quaternion.Euler(rotationAxis * clampedRotation);
        }

        public static float GetValueByRotationInRange(Quaternion rotation, float minAngleRange,
            float maxAngleRange, float to1, float to2, Vector3 rotationAxis)
        {
            rotation.eulerAngles.Scale(rotationAxis);

            var eulerAngle = rotation.eulerAngles.magnitude;
            
            if (eulerAngle > 180f)
            {
                eulerAngle -= 360f;
            }
            
            var clampedAngle = Mathf.Clamp(eulerAngle, minAngleRange, maxAngleRange);
            return Remap(clampedAngle, minAngleRange, maxAngleRange, to1, to2);
        }
    }
}