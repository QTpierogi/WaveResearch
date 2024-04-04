using UnityEngine;

namespace WaveProject.Services
{
    [CreateAssetMenu(menuName = "Project/CreateConfig/" + nameof(MainConfig), fileName = nameof(MainConfig))]
    internal class MainConfig : ScriptableObject
    {
        [field: SerializeField] public InputService InputService { get; private set; }
    }
}