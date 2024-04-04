using UnityEngine;
using WaveProject.Input;

namespace WaveProject.Services
{
    public class InputService : MonoBehaviour, IService
    {
        [SerializeField] private InputController _inputController;

        private bool _initialized;

        public InputController Controller => _inputController;
        
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