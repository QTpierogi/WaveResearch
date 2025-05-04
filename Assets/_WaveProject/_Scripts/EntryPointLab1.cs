using UnityEngine;
using WaveProject.Extensions;
using WaveProject.Services;
using WaveProject.Station;
using WaveProject.Station.PlateLogic;
using WaveProject.UserInput;

namespace WaveProject
{
    public class EntryPointLab1 : MonoBehaviour
    {
        [SerializeField] private CameraDirectionSetter _directionSetter;
        [SerializeField] private FovChanger _fovChanger;
        [SerializeField] private CameraMover _cameraMover;

        [SerializeField] private GeneratorLab1 _generator;
        [SerializeField] private ReceiverLab1 _receiver;
        [SerializeField] private CarriageStation _carriageStation;
        [SerializeField] private InsertableWaveguidesController _insertableWaveguidesController;

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
            _carriageStation.Init();
            _insertableWaveguidesController.Init();
        }
    }
}