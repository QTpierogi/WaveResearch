using System;
using TMPro;
using UnityEngine;
using WaveProject.Utility;
using Random = UnityEngine.Random;

namespace WaveProject.Station
{
    public class Generator : MonoBehaviour
    {
        [Header("Frequency settings")] 
        [Range(0, _MAX_FREQUENCY), SerializeField] private float _frequency = 8000f;

        private const float _MAX_FREQUENCY = 10000;
        [SerializeField] private Transform _frequencyHandle;

        [Header("Power settings")] [Min(0), SerializeField]
        private float _power = 78;

        [SerializeField] private float _maxPower = 100;
        [Min(0.01f), SerializeField] private float _powerStep = 1.5f;
        [SerializeField] private Transform _powerHandle;

        [Min(0), SerializeField] private float _infelicityRange = 0.05f;
        [SerializeField] private float _angleRange = 70;

        [Space] [SerializeField] private TMP_Text _textPower;
        [SerializeField] private TMP_Text _textFrequency;

        [Space] [SerializeField] private ReceivingAntenna _receivingAntenna;

        private float _rndInfelicity;

        private void OnValidate()
        {
            if (_frequency > _MAX_FREQUENCY)
            {
                _frequency = _MAX_FREQUENCY;
            }

            if (_power > _maxPower)
            {
                _power = _maxPower;
            }
        }

        private void Start()
        {
            _rndInfelicity = Random.Range(1 - _infelicityRange, 1 + _infelicityRange);

            _frequencyHandle.rotation = Utils.GetRotationInRange(_frequency, 0, _MAX_FREQUENCY,
                -_angleRange, _angleRange, Vector3.right);

            _powerHandle.rotation = Utils.GetRotationInRange(_power, 0, _maxPower,
                -_angleRange, _angleRange, Vector3.right);
            
            _receivingAntenna.SendPower(_power * _rndInfelicity * _powerStep);
            _receivingAntenna.SendFrequency(_frequency);
        }

        private void Update()
        {
            _frequency = Utils.GetValueByRotationInRange(_frequencyHandle.rotation, -_angleRange,
                _angleRange, 0, _MAX_FREQUENCY, Vector3.right);
            
            _power = Utils.GetValueByRotationInRange(_powerHandle.rotation, -_angleRange,
                _angleRange, 0, _maxPower, Vector3.right);

            _textPower.text = $"{Mathf.Round(_power)}";
            _textFrequency.text = $"{Mathf.Round(_frequency)}";
            
            _receivingAntenna.SendPower(_power * _rndInfelicity * _powerStep);
            _receivingAntenna.SendFrequency(_frequency);
        }
    }
}