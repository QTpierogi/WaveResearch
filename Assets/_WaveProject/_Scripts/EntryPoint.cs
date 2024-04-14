using UnityEngine;
using WaveProject.Extensions;
using WaveProject.Services;
using WaveProject.UserInput;
using WaveProject.UserInput.Legacy;

namespace WaveProject
{
    public class EntryPoint : MonoBehaviour
    {
        [Header("Main"), SerializeField] private MainConfig _config;
        [SerializeField] private CameraDirectionSetter _directionSetter;

        private void Awake()
        {
            var routineService = this.Get<RoutineService>();
            ServiceManager.TryAddService(routineService);

            var input = this.Get<InputController>();
            ServiceManager.TryAddService(input);
            input.SetCameraMover(_directionSetter);
        }
    }
}