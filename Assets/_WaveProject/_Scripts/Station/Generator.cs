using TMPro;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Interaction;
using WaveProject.Utility;
using Random = UnityEngine.Random;

namespace WaveProject.Station
{
    public class Generator : MonoBehaviour
    {
        [Header("Frequency settings")] 
        [Min(0), SerializeField] private float _defaultFrequency = 8000f;
        [SerializeField] private RotateInteractable _frequencyHandle;
        [SerializeField] private TMP_Text _textFrequency;

        [Header("Power settings")] 
        [Min(0), SerializeField] private float _defaultPower = 10;
        [SerializeField] private RotateInteractable _powerHandle;

        [Space] 
        [SerializeField] private ReceivingAntenna _receivingAntenna;
        
        private float _deviationRange;
        private float _randomDeviation;
        
        private float _maxFrequency;
        private float _frequencyStep;
        private float _currentFrequency;
        
        private float _maxPower;
        private float _powerStep;
        private float _currentPower;

        private void OnValidate()
        {
            if (_defaultFrequency > InteractionSettings.Data.MaxFrequency)
            {
                _defaultFrequency = InteractionSettings.Data.MaxFrequency;
            }

            if (_defaultPower > InteractionSettings.Data.MaxPower)
            {
                _defaultPower = InteractionSettings.Data.MaxPower;
            }
        }

        private void Start()
        {
            LoadData();
            
            _randomDeviation = Random.Range(1 - _deviationRange, 1 + _deviationRange);

            _frequencyHandle.Init();
            _frequencyHandle.SetDefaultValue(_defaultFrequency, 0, _maxFrequency);
            
            _powerHandle.Init();
            _powerHandle.SetDefaultValue(_defaultPower, 0, _maxPower);
        }

        private void LoadData()
        {
            _maxFrequency = InteractionSettings.Data.MaxFrequency;
            _maxPower = InteractionSettings.Data.MaxPower;
            _deviationRange = InteractionSettings.Data.DeviationRange;
            _frequencyStep = InteractionSettings.Data.FrequencyStep;
            _powerStep = InteractionSettings.Data.PowerStep;
        }

        private void Update()
        {
            _currentFrequency = Utils.RoundToIncrement(_frequencyHandle.GetValue(), _frequencyStep);
            _currentPower = Utils.RoundToIncrement(_powerHandle.GetValue(), _powerStep);
            
            SendData();
        }

        private void SendData()
        {
            _textFrequency.text = $"{Mathf.Round(_currentFrequency)}";
            
            _receivingAntenna.SendFrequency(_currentFrequency * _randomDeviation);
            _receivingAntenna.SendPowerFactor(_currentPower / (_maxPower * 0.5f) * _randomDeviation);
        }
    }
}