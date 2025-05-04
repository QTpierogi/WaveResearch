using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Interaction;
using WaveProject.Station;
using WaveProject.Station.PlateLogic.Plates;
using WaveProject.Utility;
using Random = UnityEngine.Random;

namespace WaveProject
{
    public class ReceiverLab1 : MonoBehaviour
    {
        [Min(0), SerializeField] private float _defaultScaleFactor = 1;
        [SerializeField] private RotateInteractable _scaleFactorHandle;
        [SerializeField] private RotateInteractable _zeroOffsetHandle;
        [SerializeField] private Switcher _switcher;
        [SerializeField] private Toggle _powerToggle;
        [SerializeField] private MeshRenderer _powerMeshRenderer;
        [SerializeField] private MeshRenderer _powerZeroOffsetMeshRenderer;
        [SerializeField] private CarriageStation _carriageStation;

        [Space]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _arrowAngleRange = 70;
        [SerializeField] private Transform _arrow;

        [Space]
        [SerializeField] private float waveSpeed;


        private int _speedFactor;
        private float _speedToTarget;

        public float PowerFactor { get; private set; }
        public float Frequency { get; private set; }

        private float xDistance;
        private float zDistance;

        private float _maxScaleFactor;

        private float _minZeroOffsetFactor;
        private float _maxZeroOffsetFactor;

        private bool _isEnable;

        private bool _isZeroOffset;
        private float _zeroOffset;

        private PhaseShiftPlate _phaseShiftPlate;

        private float _result;
        private float CurrentTarget => Math.Clamp(_isEnable ? _result + _zeroOffset : 0, _minZeroOffsetFactor, 100);

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

            if (_isEnable && _isZeroOffset)
                _powerZeroOffsetMeshRenderer.material.EnableKeyword("_EMISSION");
            else _powerZeroOffsetMeshRenderer.material.DisableKeyword("_EMISSION");
        }

        private void ToggleZeroSetter(bool value)
        {
            _isZeroOffset = value;

            if (_isEnable && _isZeroOffset)
                _powerZeroOffsetMeshRenderer.material.EnableKeyword("_EMISSION");
            else _powerZeroOffsetMeshRenderer.material.DisableKeyword("_EMISSION");
        }

        public void SendPowerFactor(float power)
        {
            PowerFactor = power;
        }

        public void SendFrequency(float frequency)
        {
            Frequency = frequency;
        }

        public void SendX(float x)
        {
            xDistance = x;
        }

        public void SendZ(float z)
        {
            zDistance = z;
        }


        public void Update()
        {
            _speedFactor = _isEnable ? 1 : 2;
            _speedFactor = _isZeroOffset ? 2 : 1;

            _zeroOffset = _isEnable ? _zeroOffsetHandle.GetValue() : 0;

            if (_isEnable == false) return;

            _defaultScaleFactor = _scaleFactorHandle.GetValue();

            float waveLength = waveSpeed / Frequency;

            float closedWaveLength = waveLength / (1 - Mathf.Pow((waveLength / (2f * _carriageStation.ConstStandWidth)), 2f));

            var fX = 0f;
            var fZ = 0f;

            if(_carriageStation.crossInsert)
            {
                if(_carriageStation.loopInsert)
                {
                    fZ = Mathf.Abs(Mathf.Cos(zDistance * Mathf.PI / closedWaveLength));
                    fX = 1f;
                }
                else
                {
                    fZ = Mathf.Abs(Mathf.Sin(zDistance * Mathf.PI / closedWaveLength));
                    fX = 1f;
                }
            }
            else if(_carriageStation.longInsert)
            {
                if(_carriageStation.loopInsert)
                {
                    fX = Mathf.Abs(Mathf.Cos(xDistance * 2 * Mathf.PI / closedWaveLength));
                    fZ = 1;
                }
                else
                {
                    fX = Mathf.Abs(Mathf.Sin(xDistance * 2 * Mathf.PI / closedWaveLength));
                    fZ = 1;
                }
            }

            var value = _defaultScaleFactor * PowerFactor * fX * fZ;

            _result = value * value * 100;
        }

        private IEnumerator AimForResultValue()
        {
            float currentValue = 0;
            const float maxValue = 100f;
            var minValue = _minZeroOffsetFactor;

            while (true)
            {
                var currentTarget = _isZeroOffset ? _zeroOffset : CurrentTarget;

                currentValue = Mathf.Lerp(currentValue, currentTarget, _speedFactor * _speedToTarget * Time.deltaTime);

                _text.text = $"{Math.Clamp(Math.Round(currentValue), minValue, maxValue)}";

                _arrow.rotation = Utils.GetRotationInRange(currentValue, minValue, maxValue, -_arrowAngleRange + minValue, _arrowAngleRange, Vector3.right);

                yield return null;
            }
        }
    }
}
