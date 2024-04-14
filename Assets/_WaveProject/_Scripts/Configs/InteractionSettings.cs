using UnityEngine;

namespace WaveProject.Configs
{
    [CreateAssetMenu(fileName = "InteractionSettings", menuName = "Configs/InteractionSettings", order = 0)]
    public class InteractionSettings : ScriptableObject
    {
        [field: SerializeField] public float Sensitivity { get; private set; } = 0.005f;
        [field: SerializeField] public float FovScaleFactor { get; private set; } = .5f;
        [field: SerializeField] public float FovChangingSpeed { get; private set; } = 10;
        [field: SerializeField] public float CameraMoveSpeed { get; private set; } = 2;
        
        private static InteractionSettings _instance;

        public static InteractionSettings Data =>
            _instance ??= Resources.Load<InteractionSettings>("Configs/InteractionSettings");
    }
}