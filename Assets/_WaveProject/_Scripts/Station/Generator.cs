using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WaveProject.Station
{
    public class Generator : MonoBehaviour
    {
        [Range(0, 10000), SerializeField] private float _frequency = 8000f;
        [Range(0, 100), SerializeField] private float _power = 78;
        [Min(0.01f), SerializeField] private float _powerStep = 1.5f;
        [Min(0), SerializeField] private float _infelicityRange = 0.05f;

        [Space]
        [SerializeField] private TMP_Text _textPower;
        [SerializeField] private TMP_Text _textFrequency;
        
        [Space]
        [SerializeField] private ReceivingAntenna _receivingAntenna;
        
        private float _rndInfelicity;

        private void Start() => _rndInfelicity = Random.Range(1 - _infelicityRange, 1 + _infelicityRange);

        private void Update()
        {
            _textPower.text = $"{_power}";
            _textFrequency.text = $"{_frequency}";
            _receivingAntenna.SendPower(_power * _rndInfelicity * _powerStep);
        }
    }
}