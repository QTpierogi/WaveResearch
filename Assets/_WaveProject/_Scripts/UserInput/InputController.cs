using System;
using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Services;

namespace WaveProject.UserInput
{
    public class InputController : MonoBehaviour, IService
    {
        private CameraDirectionSetter _cameraDirectionSetter;
        private IInputSubscriber _currentSubscriber;

        private ISelectable _currentPotentialSubscriber;

        private Vector2 CurrentMousePosition => Input.mousePosition;
        private Vector2 _previousMousePosition;
        private bool _returnedToCamera;

        private void Update()
        {
            SendDirection();
            SetOutline();
            TrySubscribe();
        }

        public void SetCameraMover(CameraDirectionSetter cameraDirectionSetter)
        {
            _cameraDirectionSetter = cameraDirectionSetter;
            Subscribe(_cameraDirectionSetter);
        }

        private void SendDirection()
        {
            var delta = CurrentMousePosition - _previousMousePosition;
            _currentSubscriber.CustomUpdate(delta);
            
            _previousMousePosition = CurrentMousePosition;
        }
        
        private void SetOutline()
        {
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

            // нужно в том случае, когда насильно вышли из подписчика и сразу же подписываемся на него обратно
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

            if (_currentSubscriber is not CameraDirectionSetter)
            {
                var position = _currentSubscriber.Transform.position;
                
                _cameraDirectionSetter.ChangeMoveTargetPosition(position);
                _cameraDirectionSetter.ChangeFov(position);
            }
            
            _currentSubscriber.ForceUnsubscribed += ReturnToCameraHandler;
        }

        private void ReturnToCameraHandler()
        {
            _returnedToCamera = true;
            
            _currentSubscriber.ForceUnsubscribed -= ReturnToCameraHandler;
            Subscribe(_cameraDirectionSetter);
        }
    }
}