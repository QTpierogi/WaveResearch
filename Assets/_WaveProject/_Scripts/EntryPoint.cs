using UnityEngine;
using WaveProject.Extensions;
using WaveProject.Services;
using WaveProject.UserInput;

namespace WaveProject
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private CameraDirectionSetter _directionSetter;
        [SerializeField] private FovChanger _fovChanger;

        private void Awake()
        {
            var routineService = this.Get<RoutineService>();
            ServiceManager.TryAddService(routineService);

            var input = this.Get<InputController>();
            input.SetCamera(Camera.main);
            input.SetCameraMover(_directionSetter);
            input.SetFovChanger(_fovChanger);
            
            ServiceManager.TryAddService(input);
        }
    }
}