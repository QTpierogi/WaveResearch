using UnityEngine;

namespace WaveProject.Input
{
    public interface IInputSubscriber
    {
        void SendDirection(Vector2 direction);
    }
}