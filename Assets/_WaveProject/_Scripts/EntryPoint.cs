using UnityEngine;
using WaveProject.Extensions;
using WaveProject.Services;

namespace WaveProject
{
    public class EntryPoint : MonoBehaviour
    {
        [Header("Main"), SerializeField] private MainConfig _config;

        private void Awake()
        {
            var routineService = this.Get<RoutineService>();
            ServiceManager.TryAddService(routineService);
        }
    }
}