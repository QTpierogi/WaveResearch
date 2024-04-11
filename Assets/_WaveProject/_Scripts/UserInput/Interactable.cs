using System;
using UnityEngine;

namespace WaveProject.UserInput
{
    public class Interactable : MonoBehaviour, IInputSubscriber
    {
        public event Action ForceUnsubscribe;

        public void Enable()
        {
        }

        public void Disable()
        {
        }

        public void CustomUpdate(Vector2 direction)
        {
           Debug.Log(direction);

           if (Input.GetMouseButtonDown(0))
           {
               ForceUnsubscribe?.Invoke();
           }
        }
    }
}