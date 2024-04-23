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
        private bool _returnedToCamera;
        
        private Vector2 CurrentMousePosition => Input.mousePosition;
        private Vector2 Delta => new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        private void Update()
        {
            CustomUpdate();
            SetOutline();
            TrySubscribe();
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

        private void CustomUpdate()
        {
            _currentSubscriber.CustomUpdate(Delta);
            _fovChanger.CustomUpdate();
        }

        private void SetOutline()
        {
            if (_currentSubscriber is not CameraDirectionSetter)
                return;
            
            var ray = Camera.main.ScreenPointToRay(CurrentMousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent(out ISelectable selectable))
                {
                    _currentPotentialSubscriber?.Deselect();
                    _currentPotentialSubscriber = selectable;
                    _currentPotentialSubscriber.Select();
                }
                else
                {
                    _currentPotentialSubscriber?.Deselect();
                    _currentPotentialSubscriber = null;
                }
            }
            else
            {
                _currentPotentialSubscriber?.Deselect();
                _currentPotentialSubscriber = null;
            }
        }

        private void TrySubscribe()
        {
            if (Input.GetMouseButtonDown(0) == false) return;

            // нужно в том случае, когда насильно вышли из подписчика и сразу же подписывается на него обратно
            if (_returnedToCamera)
            {
                _returnedToCamera = false;
                return;
            }
            
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
            _returnedToCamera = _currentSubscriber.OneClickInteracting == false;
            
            _currentSubscriber.ChangingFinished -= ReturnToCameraHandler;
            Subscribe(_cameraDirectionSetter);
        }
    }
}