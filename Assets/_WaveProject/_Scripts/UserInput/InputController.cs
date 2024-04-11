using System;
using UnityEngine;
using WaveProject.Services;

namespace WaveProject.UserInput
{
    public class InputController : MonoBehaviour, IService
    {
        private IInputSubscriber _mainSubscriber;
        private IInputSubscriber _currentSubscriber;
        
        private Vector2 CurrentMousePosition => Input.mousePosition;
        private Vector2 _previousMousePosition;

        private void Update()
        {
            SendDirection();
            TrySubscribe();
        }

        public void SetMainSubscriber(IInputSubscriber mainSubscriber)
        {
            _mainSubscriber = mainSubscriber;
            Subscribe(_mainSubscriber);
        }

        private void Subscribe(IInputSubscriber subscriber)
        {
            _currentSubscriber?.Disable();
            _currentSubscriber = subscriber;
            _currentSubscriber.Enable();

            _currentSubscriber.ForceUnsubscribe += ReturnToMainHandler;
        }

        private void ReturnToMainHandler()
        {
            _currentSubscriber.ForceUnsubscribe -= ReturnToMainHandler;
            Subscribe(_mainSubscriber);
        }

        private void SendDirection()
        {
            var delta = CurrentMousePosition - _previousMousePosition;
            _currentSubscriber.CustomUpdate(delta);

            _previousMousePosition = CurrentMousePosition;
        }

        private void TrySubscribe()
        {
            if (Input.GetMouseButtonDown(0) == false) return;
            
            var ray = Camera.main.ScreenPointToRay(CurrentMousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10000, Color.red);
            
            if (Physics.Raycast (ray, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent(out IInputSubscriber subscriber))
                {
                    Debug.Log (hit.transform.name);
                    Debug.Log ("hit");
                    Subscribe(subscriber);
                }
            }
        }
    }
}