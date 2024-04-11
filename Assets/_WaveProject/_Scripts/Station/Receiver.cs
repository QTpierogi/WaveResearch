using System;
using System.Collections;
using TMPro;
using UnityEngine;
using WaveProject.Utility;

namespace WaveProject.Station
{
    public class Receiver : MonoBehaviour
    {
        [Min(0), SerializeField] private float _scaleFactor = 1;
        [SerializeField] private Transform _scaleFactorHandle;
        [SerializeField] private float _scaleFactorAngleRange = 90;
        [SerializeField] private float _maxScaleFactor = 5;
        
        [SerializeField] private float _speedToTarget = 2;
        [SerializeField] private bool _isEnable;

        [Space] 
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _arrowAngleRange = 70;

        [SerializeField] private Transform _arrow;

        [Space] [SerializeField] private ReceivingAntenna _receivingAntenna;

        private float _result;
        private int _turnOn;
        private int _speedFactor;


        private void OnValidate()
        {
            if (_scaleFactor > _maxScaleFactor)
            {
                _scaleFactor = _maxScaleFactor;
            }
        }

        private void Start()
        {
            StartCoroutine(AimForResultValue());
            
            _scaleFactorHandle.rotation = Utils.GetRotationInRange(_scaleFactor, 0, _maxScaleFactor,
                -_scaleFactorAngleRange, _scaleFactorAngleRange, Vector3.right);
        }

        private float CurrentTarget => _result * _turnOn;

        private void Update()
        {
            _turnOn = _isEnable ? 1 : 0;
            _speedFactor = _isEnable ? 1 : 2;
            
            _scaleFactor = Utils.GetValueByRotationInRange(_scaleFactorHandle.rotation, -_scaleFactorAngleRange,
                _scaleFactorAngleRange, 0, _maxScaleFactor, Vector3.right);

            var power = _receivingAntenna.Power;
            var distanceFactor = _receivingAntenna.GetAntennasDistanceFactor();
            var angleInRadians = GetAngleInRadians(_receivingAntenna.transform.rotation.eulerAngles.z);
            var angleCosine = Mathf.Abs(Mathf.Cos(angleInRadians));
            var value = _scaleFactor * distanceFactor * power * angleCosine;

            var clampedValue = Mathf.Clamp(value, 0, 100);
            _result = clampedValue;
        }

        public void ToggleEnabling(bool value) => _isEnable = value;

        private IEnumerator AimForResultValue()
        {
            float currentValue = 0;
            while (true)
            {
                currentValue = Mathf.Lerp(currentValue, CurrentTarget, _speedFactor * _speedToTarget * Time.deltaTime);

                _text.text = $"{Mathf.Round(currentValue)}";
                _arrow.rotation = Utils.GetRotationInRange(currentValue, 0, 100, -_arrowAngleRange, _arrowAngleRange, Vector3.right);

                yield return null;
            }
        }

        private float GetAngleInRadians(float angleInDegree)
        {
            return angleInDegree * Mathf.PI / 180;
        }
    }
}