using UnityEngine;

namespace WaveProject.Input
{
    public class Interactable : MonoBehaviour, IInputSubscriber
    {
        public void SendDirection(Vector2 direction)
        {
           Debug.Log(direction); 
        }
    }
}