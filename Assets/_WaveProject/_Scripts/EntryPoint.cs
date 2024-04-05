using UnityEngine;
using WaveProject.Extensions;
using WaveProject.Input;
using WaveProject.Services;

namespace WaveProject
{
    public class EntryPoint : MonoBehaviour
    {
        [Header("Main"), SerializeField] private MainConfig _config;
        [SerializeField] private Interactable _interactable;
        

        private void Awake()
        {
            var routineService = this.Get<RoutineService>();
            ServiceManager.TryAddService(routineService);

            var input = this.Get<MouseInputController>();
            ServiceManager.TryAddService(input);
        }
    }
}