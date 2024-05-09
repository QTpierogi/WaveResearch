using System;
using System.Collections;
using TMPro;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Interaction;
using WaveProject.Station.Plates;
using WaveProject.Utility;

namespace WaveProject.Station
{
    public class Receiver : MonoBehaviour
    {
        [Min(0), SerializeField] private float _defaultScaleFactor = 1;
        [SerializeField] private RotateInteractable _scaleFactorHandle;
        [SerializeField] private Toggle _toggle;

        [Space] 
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _arrowAngleRange = 70;
        [SerializeField] private Transform _arrow;

        [Space] 
        [SerializeField] private ReceivingAntenna _receivingAntenna;

        private float _result;
        private int _turnOn;
        private int _speedFactor;
        
        private float _maxScaleFactor;
        private bool _isEnable;
        private float _speedToTarget;
        
        private PhaseShiftPlate _phaseShiftPlate;

        private float CurrentTarget => _result * _turnOn;

        private void OnValidate()
        {
            if (_defaultScaleFactor > InteractionSettings.Data.MaxReceiverScaleFactor)
            {
                _defaultScaleFactor = InteractionSettings.Data.MaxReceiverScaleFactor;
            }
        }

        private void Start()
        {
            LoadData();
            StartCoroutine(AimForResultValue());
            
            _scaleFactorHandle.Init();
            _scaleFactorHandle.SetDefaultValue(_defaultScaleFactor, 0, _maxScaleFactor);
            
            _toggle.Init();
            _toggle.SetDefaultToggledState(false);
            
            _phaseShiftPlate = new EmptyPhaseShiftPlate(0, 0);
        }

        private void LoadData()
        {
            _maxScaleFactor = InteractionSettings.Data.MaxReceiverScaleFactor;
            _speedToTarget = InteractionSettings.Data.ReceiverArrowSpeedToTarget;
        }

        public void ToggleEnabling(bool value) => _isEnable = value;

        public void SetPhaseShiftPlate(PlateType type, float plateLength, float plateThickness, float plateResistance)
        {
            var plateLengthInMeters = Utils.MillimetersToMeters(plateLength);
            var plateThicknessInMeters = Utils.MillimetersToMeters(plateThickness);
            
            switch (type)
            {
                case PlateType.None:
                    _phaseShiftPlate = new EmptyPhaseShiftPlate(plateLengthInMeters, plateThicknessInMeters);
                    break;
                
                case PlateType.Metal:
                    _phaseShiftPlate = new MetalPhaseShiftPlate(plateLengthInMeters, plateThicknessInMeters);
                    break;
                
                case PlateType.Dielectric:
                    _phaseShiftPlate = new DielectricPhaseShiftPlate(plateLengthInMeters, plateThicknessInMeters, plateResistance);
                    break;
            }
        }

        private void Update()
        {
            _turnOn = _isEnable ? 1 : 0;
            _speedFactor = _isEnable ? 1 : 2;
            
            _defaultScaleFactor = _scaleFactorHandle.GetValue();

            var distanceFactor = _receivingAntenna.GetAntennasDistanceFactor();
            var powerFactor = _receivingAntenna.PowerFactor;
            
            var frequency = _receivingAntenna.Frequency;
            
            var angleInDegree = _receivingAntenna.GetRotation();
            var angleInRadians = Utils.DegreeToRadians(angleInDegree);

            var variantWavelength = _phaseShiftPlate.GetVariantWavelength(Utils.MHzToHz(frequency));
            var receiverSignalLevel = _phaseShiftPlate.GetReceiverSignalLevel(angleInRadians, variantWavelength);
            
            var value = _defaultScaleFactor * distanceFactor * powerFactor * receiverSignalLevel;

            var clampedValue = Math.Clamp(value, 0, 100);
            _result = (float)clampedValue;
        }
        
        private IEnumerator AimForResultValue()
        {
            double currentValue = 0;
            while (true)
            {
                currentValue = Mathf.Lerp((float)currentValue, CurrentTarget, _speedFactor * _speedToTarget * Time.deltaTime);

                _text.text = $"{Math.Round(currentValue)}";
                _arrow.rotation = Utils.GetRotationInRange((float)currentValue, 0, 100, -_arrowAngleRange, _arrowAngleRange, Vector3.right);

                yield return null;
            }
        }
    }
}