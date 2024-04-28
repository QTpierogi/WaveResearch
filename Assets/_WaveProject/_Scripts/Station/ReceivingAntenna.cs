using System;
using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Utility;

namespace WaveProject.Station
{
    public class ReceivingAntenna : MonoBehaviour
    {
        [SerializeField] private float _baseDistance = 4;
        [SerializeField] private MoveInteractable _secondAntenna;
        [SerializeField] private InfiniteRotateInteractable _rotatePart;
        
        public float PowerFactor { get; private set; }
        public float Frequency { get; private set; }

        private void OnValidate()
        {
            _baseDistance = Vector3.Distance(_secondAntenna.Transform.position, transform.position);
        }

        private void Start()
        {
            _secondAntenna.Init();
            _secondAntenna.SetDefaultValue();
            
            _rotatePart.Init();
            _rotatePart.SetDefaultRotation();
        }

        public float GetAntennasDistanceFactor()
        {
          return 1 * _baseDistance / Vector3.Distance(_secondAntenna.Transform.position, transform.position);
        }

        public float GetRotation() => _rotatePart.GetRotation();

        public void SendPowerFactor(float power)
        {
            PowerFactor = power;
        }

        public void SendFrequency(float frequency)
        {
            Frequency = frequency;
        }
    }
}