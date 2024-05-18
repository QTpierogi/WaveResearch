using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Services;

namespace WaveProject.UserInput
{
    public class InputController : MonoBehaviour, IService
    {
        private CameraDirectionSetter _cameraDirectionSetter;
        private FovChanger _fovChanger;
        
        private IInputSubscriber _currentSubscriber;
        private ISelectable _currentPotentialSubscriber;

        private Vector2 _previousMousePosition;
        private Camera _camera;

        private static Vector2 CurrentMousePosition => Input.mousePosition;
        private static Vector2 Delta => new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        private const float _RAYCAST_DISTANCE = 1000;

        private void Update()
        {
            CustomUpdate();
            FindOutliner();
            TrySubscribe();
        }
        
        public void SetCamera(Camera cam)
        {
            _camera = cam;
        }

        public void SetCameraMover(CameraDirectionSetter cameraDirectionSetter)
        {
            _cameraDirectionSetter = cameraDirectionSetter;
            Subscribe(_cameraDirectionSetter);
        }

        public void SetFovChanger(FovChanger fovChanger)
        {
            _fovChanger = fovChanger;
        }

        public void ExternSubscribe(IInputSubscriber externSubscriber)
        {
            if (externSubscriber is ISelectable selectable)
                SetOutline(selectable);
            else ClearOutline();
            
            Subscribe(externSubscriber);
        }

        private void CustomUpdate()
        {
            _currentSubscriber.CustomUpdate(Delta);
            _fovChanger.CustomUpdate();
        }

        private void FindOutliner()
        {
            if (_currentSubscriber is not CameraDirectionSetter)
                return;
            
            var ray = _camera.ScreenPointToRay(CurrentMousePosition);

            if (Physics.Raycast(ray, out var hit, _RAYCAST_DISTANCE,IInputSubscriber.LayerMask))
            {
                if (hit.collider.TryGetComponent(out ISelectable selectable))
                {
                    SetOutline(selectable);
                }
                else
                {
                    ClearOutline();
                }
            }
            else
            {
                ClearOutline();
            }
        }

        private void ClearOutline()
        {
            _currentPotentialSubscriber?.Deselect();
            _currentPotentialSubscriber = null;
        }

        private void SetOutline(ISelectable selectable)
        {
            _currentPotentialSubscriber?.Deselect();
            _currentPotentialSubscriber = selectable;
            _currentPotentialSubscriber.Select();
        }

        private void TrySubscribe()
        {
            if (Input.GetMouseButtonDown(0) == false) return;
            
            if (_currentPotentialSubscriber is IInputSubscriber subscriber)
            {
                Subscribe(subscriber);
            }
        }

        private void Subscribe(IInputSubscriber subscriber)
        {
            _currentSubscriber?.Disable();
            _currentSubscriber = subscriber;
            _currentSubscriber.Enable();

            // if (_currentSubscriber is not CameraDirectionSetter)
            // {
            //     // var position = _currentSubscriber.Transform.position;
            //     
            //     // _cameraDirectionSetter.ChangeMoveTargetPosition(position);
            //     // _cameraDirectionSetter.ChangeFov(position);
            // }
            
            _currentSubscriber.ChangingFinished += ReturnToCameraHandler;
        }

        private void ReturnToCameraHandler()
        {
            _currentSubscriber.ChangingFinished -= ReturnToCameraHandler;
            Subscribe(_cameraDirectionSetter);
        }
    }
}