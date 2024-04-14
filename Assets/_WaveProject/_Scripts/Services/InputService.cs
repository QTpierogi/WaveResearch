using UnityEngine;
using UnityEngine.Serialization;
using WaveProject.UserInput;
using WaveProject.UserInput.Legacy;

namespace WaveProject.Services
{
    public class InputService : MonoBehaviour, IService
    {
        [FormerlySerializedAs("_inputController")] [SerializeField] private CameraController _cameraController;

        private bool _initialized;

        public CameraController Controller => _cameraController;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Init(Transform newParent)
        {
            if (_initialized)
            {
                Debug.LogWarning($"The {GetType().Name} is already inited");
                return;
            }
        
            _initialized = true;
            SetObjectsParent(newParent);
        }

        private void SetObjectsParent(Transform newParent)
        {
            Controller.transform.SetParent(newParent, false);
        }
    }
}