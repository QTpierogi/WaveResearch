using System;
using System.Collections.Generic;
using UnityEngine;
using WaveProject.Services;

namespace WaveProject.Input
{
    public class MouseInputController : MonoBehaviour, IService
    {
        private List<IInputSubscriber> _subscribers = new();
        
        private Vector2 CurrentMousePosition => UnityEngine.Input.mousePosition;
        private Vector2 _previousMousePosition;

        private void Update()
        {
            SendDirection();
            TrySubscribe();
        }

        private void SendDirection()
        {
            if (_subscribers.Count == 0)
                return;

            var delta = CurrentMousePosition - _previousMousePosition;
            var normalizedDelta = delta.normalized;
            foreach (var subscriber in _subscribers)
            {
                subscriber.SendDirection(normalizedDelta);
            }

            _previousMousePosition = CurrentMousePosition;
        }

        private void TrySubscribe()
        {
            // if (UnityEngine.Input.GetMouseButtonDown(0) == false)
            //     return;
            
            var ray = UnityEngine.Camera.main.ScreenPointToRay (CurrentMousePosition);
            
            Debug.DrawRay(ray.origin, ray.direction);
            
            Ray ray = new Ray(transform.position, (secondHidePoint.transform.position - transform.position).normalized * rayMaxDistance);
            Debug.DrawRay(ray.origin, ray.direction * rayMaxDistance, Color.red);

            
            if (Physics.Raycast (ray, out var hit)) 
            {
                if (hit.transform.gameObject.TryGetComponent(out Interactable interactable))
                {
                    Debug.Log (hit.transform.name);
                    Debug.Log ("hit");
                    Subscribe(interactable);
                }
            }
        }

        public void Subscribe(IInputSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }
    }
}