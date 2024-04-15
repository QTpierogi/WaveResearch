using System;
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Utility;
using Vector3 = UnityEngine.Vector3;

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
        
        private float _internalWaveguideWidth;
        private float _speedOfLight;

        private float CurrentTarget => _result * _turnOn;

        private void OnValidate()
        {
            if (_scaleFactor > _maxScaleFactor)
            {
                _scaleFactor = _maxScaleFactor;
            }
        }

        private void Start()
        {
            LoadData();
            StartCoroutine(AimForResultValue());
            
            _scaleFactorHandle.rotation = Utils.GetRotationInRange(_scaleFactor, 0, _maxScaleFactor,
                -_scaleFactorAngleRange, _scaleFactorAngleRange, Vector3.right);
        }

        private void LoadData()
        {
            _internalWaveguideWidth = InteractionSettings.INTERNAL_WAVEGUIDE_WIDTH;
            _speedOfLight = InteractionSettings.SPEED_OF_LIGHT;
        }

        public void ToggleEnabling(bool value) => _isEnable = value;

        private void Update()
        {
            _turnOn = _isEnable ? 1 : 0;
            _speedFactor = _isEnable ? 1 : 2;
            
            _scaleFactor = Utils.GetValueByRotationInRange(_scaleFactorHandle.rotation, -_scaleFactorAngleRange,
                _scaleFactorAngleRange, 0, _maxScaleFactor, Vector3.right);

            var power = _receivingAntenna.Power;
            var frequency = _receivingAntenna.Frequency;
            
            var distanceFactor = _receivingAntenna.GetAntennasDistanceFactor();
            var angleInDegree = _receivingAntenna.transform.rotation.eulerAngles.z;
            var angleInRadians = Utils.DegreeToRadians(angleInDegree);
            var angleCosine = Mathf.Abs(Mathf.Cos(angleInRadians));
            // var angleCosine = Mathf.Abs((Mathf.Cos(angleInRadians) + 1) / 2);

            var variantWavelength = GetVariantWavelength(Utils.MHzToHz(frequency));
            var plateLength = Utils.MillimetersToMeters(23); 
            var plateThickness = Utils.MillimetersToMeters(3); 
            
            // var angleCosine = GetReceiverSignalLevel(variantWavelength, plateLength, plateThickness);
            
            var value = _scaleFactor * distanceFactor * power * angleCosine;

            var clampedValue = Mathf.Clamp(value, 0, 100);
            _result = clampedValue;
        }

        private float GetReceiverSignalLevel(float variantWavelength, float plateLength, float plateThickness)
        {
            var betta = 30; //GetPolarizationCharacteristicInclinationAngle(variantWavelength, plateLength, plateThickness);
            var r = .7f; //GetEllipticityCoefficient(variantWavelength, plateLength, plateThickness);

            return Mathf.Pow(Mathf.Abs(0 - betta) * (1 - r) + r, 2) / 100; // * 100;
        }
        
        private float GetEllipticityCoefficient(float variantWavelength, float plateLength, float plateThickness)
        {
            var a = GetA(variantWavelength, plateThickness);
            
            var wavelength10 = GetWavelength10(variantWavelength);
            var wavelength01 = GetWavelength01(variantWavelength, plateThickness);
            
            var shift = GetPhaseShift(plateLength, wavelength10, wavelength01);


            return (2*a * Mathf.Abs(Mathf.Sin(shift))) /
                   (a*a + 1 + Mathf.Sqrt(Mathf.Pow(a, 4) - 2 * a*a + 1 + 4*a*a * Mathf.Pow(Mathf.Cos(shift), 2)));
        }
        
        private float GetPolarizationCharacteristicInclinationAngle(float variantWavelength, float plateLength, float plateThickness)
        {
            var a = GetA(variantWavelength, plateThickness);
            var wavelength10 = GetWavelength10(variantWavelength);
            var wavelength01 = GetWavelength01(variantWavelength, plateThickness);
            
            var shift = GetPhaseShift(plateLength, wavelength10, wavelength01);
                
            return 0.5f * Mathf.Atan((2 * a) / (a*a - 1) * Mathf.Cos(shift));
        }

        private float GetA(float variantWavelength, float plateThickness)
        {
            var g = GetG(variantWavelength, plateThickness);
            
            return Mathf.Sqrt(1 - Mathf.Pow(g, 2));
        }

        private float GetG(float variantWavelength, float plateThickness)
        {
            var z10 = GetZ10(variantWavelength);
            var zIn = GetZIn(variantWavelength, plateThickness);

            return 
                z10 - zIn /
                z10 + zIn;
        }

        private float GetZIn(float variantWavelength, float plateThickness)
        {
            const float pi = Mathf.PI;
            var a = _internalWaveguideWidth;

            var z01 = GetZ01(variantWavelength, plateThickness);
            var z10 = GetZ10(variantWavelength);

            var lambda01 = GetWavelength01(variantWavelength, plateThickness);

            var tanArg = 2 * pi * variantWavelength / lambda01;

            var complex = new Complex(0, 1);
            
            return z01 * 
                   (z10 + (float)complex.Imaginary * z01 * Mathf.Tan(tanArg)) / 
                   (z01 + (float)complex.Imaginary * z10 * Mathf.Tan(tanArg));
        }

        private float GetZ10(float variantWavelength)
        {
            const float pi = Mathf.PI;
            var a = _internalWaveguideWidth;
            
            return 120 * pi / Mathf.Sqrt(1 - Mathf.Pow(variantWavelength / (2 * a), 2));
        }

        private float GetZ01(float variantWavelength, float plateThickness)
        {
            const float pi = Mathf.PI;
            var a = _internalWaveguideWidth;


            return 120 * pi / Mathf.Sqrt(1 - Mathf.Pow(variantWavelength / (2 * (a - plateThickness)), 2));
        }

        private float GetWavelength10(float variantWavelength)
        {
            var a = _internalWaveguideWidth;

            return variantWavelength / Mathf.Sqrt(1 - Mathf.Pow(variantWavelength / 2 * a, 2));
        }

        private float GetWavelength01(float variantWavelength, float plateThickness)
        {
            var a = _internalWaveguideWidth;

            return variantWavelength / Mathf.Sqrt(1 - Mathf.Pow(variantWavelength / 2 * (a - plateThickness), 2));
        }

        private float GetPhaseShift(float plateLength, float wavelength10, float wavelength01)
        {
            const float pi = Mathf.PI;

            return 2 * pi / wavelength10 * plateLength - 2 * pi / wavelength01 * plateLength;
        }

        private float GetVariantWavelength(float frequency)
        {
            return _speedOfLight / frequency;
        }

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
    }
}