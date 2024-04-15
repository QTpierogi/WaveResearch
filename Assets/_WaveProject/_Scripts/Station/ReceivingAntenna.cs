using System;
using UnityEngine;

namespace WaveProject.Station
{
    public class ReceivingAntenna : MonoBehaviour
    {
        [SerializeField] private float _baseDistance = 4;
        [SerializeField] private Transform _secondAntenna;
        
        public float Power { get; private set; }
        public float Frequency { get; private set; }

        private void OnValidate()
        {
            _baseDistance = Vector3.Distance(_secondAntenna.position, transform.position);
        }

        public float GetAntennasDistanceFactor()
        {
          return 1 * _baseDistance / Vector3.Distance(_secondAntenna.position, transform.position);
        }

        public void SendPower(float power)
        {
            Power = power;
        }

        public void SendFrequency(float frequency)
        {
            Frequency = frequency;
        }
    }
}