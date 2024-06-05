using UnityEngine;
using WaveProject.Extensions;
using WaveProject.Services;
using WaveProject.Station;
using WaveProject.Station.PlateLogic;
using WaveProject.UserInput;

namespace WaveProject
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private CameraDirectionSetter _directionSetter;
        [SerializeField] private FovChanger _fovChanger;
        [SerializeField] private CameraMover _cameraMover;

        [SerializeField] private Generator _generator;
        [SerializeField] private Receiver _receiver;
        [SerializeField] private ReceivingAntenna _receivingAntenna;
        [SerializeField] private PlateGenerator _plateGenerator;

        private void Awake()
        {
            var routineService = this.Get<RoutineService>();
            ServiceManager.TryAddService(routineService);

            var input = this.Get<InputController>();
            input.SetCamera(Camera.main);
            input.SetCameraDirectionMover(_directionSetter);
            input.SetFovChanger(_fovChanger);
            input.SetCameraMover(_cameraMover);
            
            ServiceManager.TryAddService(input);
            
            _generator.Init();
            _receiver.Init();
            _receivingAntenna.Init();
            _plateGenerator.Init();
        }
    }
}