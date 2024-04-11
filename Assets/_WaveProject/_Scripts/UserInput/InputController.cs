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

        private void Subscribe(IInputSubscriber subscriber)
        {
            _currentSubscriber?.Disable();
            _currentSubscriber = subscriber;
            _currentSubscriber.Enable();

            _currentSubscriber.ForceUnsubscribe += ReturnToCameraHandler;
        }

        private void ReturnToCameraHandler()
        {
            _currentSubscriber.ForceUnsubscribe -= ReturnToCameraHandler;
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
            
            if (_currentPotentialSubscriber is IInputSubscriber subscriber)
            {
                Subscribe(subscriber);
            }
        }
    }
}