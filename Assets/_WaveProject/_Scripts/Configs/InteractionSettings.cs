using UnityEngine;

namespace WaveProject.Configs
{
    [CreateAssetMenu(fileName = "InteractionSettings", menuName = "Configs/InteractionSettings", order = 0)]
    public class InteractionSettings : ScriptableObject
    {
        [field:Header("Interactable")]
        [field: SerializeField] public float Sensitivity { get; private set; } = 0.005f;
        
        [field:Header("Camera Fov")]
        [field: SerializeField] public float FovMinValue { get; private set; } = 20;
        [field: SerializeField] public float FovDefaultValue { get; private set; } = 60;
        [field: SerializeField] public float FovChangingSpeed { get; private set; } = 10;
        [field: SerializeField] public float FovSensitivity { get; private set; } = 3;

        [field:Header("Camera Movement")]
        [field: SerializeField] public float CameraMoveSpeed { get; private set; } = 2;

        // внутрянняя ширина волновода в метрах
        public const float INTERNAL_WAVEGUIDE_WIDTH = 0.023f; 

        // скорость света в метрах == 3 * 10^8
        public const float SPEED_OF_LIGHT = 3 * 100000000F; 
        
        private static InteractionSettings _instance;

        public static InteractionSettings Data =>
            _instance ??= Resources.Load<InteractionSettings>("Configs/InteractionSettings");
    }
}