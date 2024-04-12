using UnityEngine;

namespace WaveProject.Utility
{
    public static class Easings
    {
        public static float OutCubic(float x)
        {
            return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
        }
    }
}