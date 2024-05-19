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
        [SerializeField] private InfiniteRotateInteractable _rotatePart;
        [SerializeField] private TMP_Text _rotationText;
        
        public float PowerFactor { get; private set; }
        public float Frequency { get; private set; }

        private void OnValidate()
        {
            _baseDistance = Vector3.Distance(_secondAntenna.Transform.position, transform.position);
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