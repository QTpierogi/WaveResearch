using System;
using TMPro;
using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Utility;

namespace WaveProject.Station
{
    public class ReceivingAntenna : MonoBehaviour
    {
        [SerializeField] private float _baseDistance = 4;
        [SerializeField] private MoveBetweenPointsInteractable _secondAntenna;
        
        [SerializeField] private Transform _thisAntennaDistancePoint;
        [SerializeField] private Transform _secondAntennaDistancePoint;
        
        [SerializeField] private InfiniteRotateInteractable _rotatePart;
        [SerializeField] private TMP_Text _rotationText;
        
        public float PowerFactor { get; private set; }
        public float Frequency { get; private set; }

        private void OnValidate()
        {
            _baseDistance = GetPointsDistance();
        }

        public void Init()
        {
            _secondAntenna.Init();
            _secondAntenna.SetDefaultValue();
            
            _rotatePart.Init();
            _rotatePart.SetDefaultRotation();
        }

        private void Update()
        {
            _rotationText.text = $"{MathF.Round(GetRotation())}";
        }

        public float GetAntennasDistanceFactor()
        {
          return 1 * _baseDistance / Mathf.Pow(GetPointsDistance(), 2);
        }

        private float GetPointsDistance()
        {
            return Vector3.Distance(_thisAntennaDistancePoint.position, _secondAntennaDistancePoint.position);
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