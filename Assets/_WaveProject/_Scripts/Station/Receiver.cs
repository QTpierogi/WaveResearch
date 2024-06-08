using System;
using System.Collections;
using TMPro;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Interaction;
using WaveProject.Station.PlateLogic.Plates;
using WaveProject.Utility;
using Random = UnityEngine.Random;

namespace WaveProject.Station
{
    public class Receiver : MonoBehaviour
    {
        [Min(0), SerializeField] private float _defaultScaleFactor = 1;
        [SerializeField] private RotateInteractable _scaleFactorHandle;
        [SerializeField] private RotateInteractable _zeroOffsetHandle;
        [SerializeField] private Switcher _switcher;
        [SerializeField] private Toggle _powerToggle;
        [SerializeField] private MeshRenderer _powerMeshRenderer;

        [Space] 
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _arrowAngleRange = 70;
        [SerializeField] private Transform _arrow;

        [Space] 
        [SerializeField] private ReceivingAntenna _receivingAntenna;

        private int _speedFactor;
        private float _speedToTarget;
        
        private float _maxScaleFactor;
        
        private float _minZeroOffsetFactor;
        private float _maxZeroOffsetFactor;
        
        private bool _isEnable;

        private bool _isZeroOffset;
        private float _zeroOffset;
        
        private PhaseShiftPlate _phaseShiftPlate;

        private float _result;
        private float CurrentTarget => Math.Clamp(_result * (_isEnable ? 1 : 0), 0, 100) + _zeroOffset;

        private void OnValidate()
        {
            if (_defaultScaleFactor > InteractionSettings.Data.MaxReceiverScaleFactor)
            {
                _defaultScaleFactor = InteractionSettings.Data.MaxReceiverScaleFactor;
            }
        }

        public void Init()
        {
            const bool defaultZeroSetter = false;
            const bool defaultPower = false;
            
            LoadData();
            StartCoroutine(AimForResultValue());

            _zeroOffset = Random.Range(_minZeroOffsetFactor, _maxZeroOffsetFactor);
            
            _scaleFactorHandle.Init();
            _scaleFactorHandle.SetDefaultValue(_defaultScaleFactor, 0, _maxScaleFactor);
            
            _zeroOffsetHandle.Init();
            _zeroOffsetHandle.SetDefaultValue(_zeroOffset, _minZeroOffsetFactor, _maxZeroOffsetFactor);
            
            _switcher.Init();
            _switcher.SetDefaultToggledState(defaultZeroSetter);
            _switcher.Toggled.AddListener(ToggleZeroSetter);
            ToggleZeroSetter(defaultZeroSetter);
            
            _powerToggle.Init();
            _powerToggle.SetDefaultToggledState(defaultPower);
            _powerToggle.Toggled.AddListener(ToggleEnabling);
            ToggleEnabling(defaultPower);
            
            _phaseShiftPlate = new EmptyPhaseShiftPlate(0, 0);
        }

        private void LoadData()
        {
            _maxScaleFactor = InteractionSettings.Data.MaxReceiverScaleFactor;
            _minZeroOffsetFactor = InteractionSettings.Data.MinZeroOffsetFactor;
            _maxZeroOffsetFactor = InteractionSettings.Data.MaxZeroOffsetFactor;
            _speedToTarget = InteractionSettings.Data.ReceiverArrowSpeedToTarget;
        }

        private void ToggleEnabling(bool value)
        {
            _isEnable = value;
            
            if (_isEnable) 
                _powerMeshRenderer.material.EnableKeyword("_EMISSION");
            else _powerMeshRenderer.material.DisableKeyword("_EMISSION");
        }

        private void ToggleZeroSetter(bool value)
        {
            _isZeroOffset = value;
        }

        public void SetPhaseShiftPlate(PlateType type, float plateLength = 0, float plateThickness = 0, float plateResistance = 0)
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
            _speedFactor = _isEnable ? 1 : 2;
            _speedFactor = _isZeroOffset ? 2 : 1;
            
            _defaultScaleFactor = _scaleFactorHandle.GetValue();
            _zeroOffset = _zeroOffsetHandle.GetValue();

            var distanceFactor = _receivingAntenna.GetAntennasDistanceFactor();
            var powerFactor = _receivingAntenna.PowerFactor;
            
            var frequency = _receivingAntenna.Frequency;
            
            var angleInDegree = _receivingAntenna.GetRotation();
            var angleInRadians = Utils.DegreeToRadians(angleInDegree);

            var variantWavelength = _phaseShiftPlate.GetVariantWavelength(Utils.MHzToHz(frequency));
            var receiverSignalLevel = _phaseShiftPlate.GetReceiverSignalLevel(angleInRadians, variantWavelength);
            
            var value = _defaultScaleFactor * distanceFactor * powerFactor * receiverSignalLevel;

            _result = (float)value;
        }
        
        private IEnumerator AimForResultValue()
        {
            float currentValue = 0;
            var minValue = 0f;
            var maxValue = 100f;

            while (true)
            {
                float currentTarget;
                if (_isZeroOffset)
                {
                    currentTarget = _zeroOffset;
                    minValue = _minZeroOffsetFactor;
                }
                else
                {
                    currentTarget = CurrentTarget;
                }
                
                currentValue = Mathf.Lerp(currentValue, currentTarget, _speedFactor * _speedToTarget * Time.deltaTime);

                _text.text = $"{Math.Round(currentValue)}";

                _arrow.rotation = Utils.GetRotationInRange(currentValue, minValue, maxValue, -_arrowAngleRange + minValue, _arrowAngleRange, Vector3.right);

                yield return null;
            }
        }
    }
}