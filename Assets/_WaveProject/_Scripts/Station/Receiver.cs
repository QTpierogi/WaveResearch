using System;
using System.Collections;
using System.Numerics;
using TMPro;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Interaction;
using WaveProject.Utility;
using Vector3 = UnityEngine.Vector3;

namespace WaveProject.Station
{
    public class Receiver : MonoBehaviour
    {
        [Min(0), SerializeField] private float _defaultScaleFactor = 1;
        [SerializeField] private RotateInteractable _scaleFactorHandle;

        [Space] 
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _arrowAngleRange = 70;
        [SerializeField] private Transform _arrow;

        [Space] 
        [SerializeField] private ReceivingAntenna _receivingAntenna;
        
        [SerializeField] private float _plateLenghtInMM = 30;
        [SerializeField] private float _plateThicknessInMM = 3;

        private float _result;
        private int _turnOn;
        private int _speedFactor;
        
        private float _internalWaveguideWidth;
        private float _speedOfLight;
        private float _maxScaleFactor;
        private bool _isEnable;
        private float _speedToTarget;

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
        }

        private void LoadData()
        {
            _maxScaleFactor = InteractionSettings.Data.MaxReceiverScaleFactor;
            _speedToTarget = InteractionSettings.Data.ReceiverArrowSpeedToTarget;
            
            _internalWaveguideWidth = InteractionSettings.INTERNAL_WAVEGUIDE_WIDTH;
            _speedOfLight = InteractionSettings.SPEED_OF_LIGHT;
        }

        public void ToggleEnabling(bool value) => _isEnable = value;

        private void Update()
        {
            _turnOn = _isEnable ? 1 : 0;
            _speedFactor = _isEnable ? 1 : 2;
            
            _defaultScaleFactor = _scaleFactorHandle.GetValue();

            var power = _receivingAntenna.Power;
            var frequency = _receivingAntenna.Frequency;
            
            var distanceFactor = _receivingAntenna.GetAntennasDistanceFactor();
            var angleInDegree = _receivingAntenna.GetRotation();
            var angleInRadians = Utils.DegreeToRadians(angleInDegree);

            var variantWavelength = GetVariantWavelength(Utils.MHzToHz(frequency));
            var plateLength = Utils.MillimetersToMeters(_plateLenghtInMM);
            var plateThickness = Utils.MillimetersToMeters(_plateThicknessInMM);
            
            var angleCosine = GetReceiverSignalLevel(angleInRadians, variantWavelength, plateLength, plateThickness);
            
            var value = _defaultScaleFactor * distanceFactor/* * power*/ * angleCosine;

            var clampedValue = Math.Clamp(value, 0, 100);
            _result = (float)clampedValue;
        }

        private double GetIdealPlateLength(double variantWavelength, double plateThickness)
        {
            var l10 = GetWavelength10(variantWavelength);
            var l01 = GetWavelength01(variantWavelength, plateThickness);
            return (l10*l01) / 4 * Math.Abs(l10 - l01);
        }

        private double GetReceiverSignalLevel(float angleInRadians, double variantWavelength, double plateLength,
            double plateThickness)
        {
            var betta = 0;GetPolarizationCharacteristicInclinationAngle(variantWavelength, plateLength, plateThickness);
            var r = 0;GetEllipticityCoefficient(variantWavelength, plateLength, plateThickness);

            var cosOfAngle = Math.Cos(angleInRadians - betta);
            return Math.Pow(Math.Abs(cosOfAngle) * (1 - r) + r, 2) * 100;
        }
        
        private double GetEllipticityCoefficient(double variantWavelength, double plateLength, double plateThickness)
        {
            var a = GetA(variantWavelength, plateLength, plateThickness);
            
            var wavelength10 = GetWavelength10(variantWavelength);
            var wavelength01 = GetWavelength01(variantWavelength, plateThickness);
            
            var shift = GetPhaseShift(plateLength, wavelength10, wavelength01);
            
            var absSinOfShift = Math.Abs(Math.Sin(shift));
            var powSinOfShift = Math.Pow(Math.Cos(shift), 2);

            var aPow4 = Math.Pow(a, 4);

            return (2*a * absSinOfShift) /
                   (a*a + 1 + Math.Sqrt(aPow4 - 2 * a*a + 1 + 4*a*a * powSinOfShift));
        }
        
        private double GetPolarizationCharacteristicInclinationAngle(double variantWavelength, double plateLength, double plateThickness)
        {
            var a = GetA(variantWavelength, plateLength, plateThickness);
            var wavelength10 = GetWavelength10(variantWavelength);
            var wavelength01 = GetWavelength01(variantWavelength, plateThickness);
            
            var shift = GetPhaseShift(plateLength, wavelength10, wavelength01);
                
            return 0.5d * Math.Atan((2 * a) / (a*a - 1) * Math.Cos(shift));
        }

        private double GetA(double variantWavelength, double plateLength, double plateThickness)
        {
            var g = GetG(variantWavelength, plateLength, plateThickness);
            
            return Math.Sqrt(1 - Math.Pow(g, 2));
        }

        private double GetG(double variantWavelength, double plateLength, double plateThickness)
        {
            var z10 = GetZ10(variantWavelength);
            var zIn = GetZIn(variantWavelength, plateLength, plateThickness);

            return
                (z10 - zIn) /
                (z10 + zIn);
        }

        private double GetZIn(double variantWavelength, double plateLength, double plateThickness)
        {
            const double pi = Mathf.PI;

            var z01 = GetZ01(variantWavelength, plateThickness);
            var z10 = GetZ10(variantWavelength);

            var lambda01 = GetWavelength01(variantWavelength, plateThickness);

            var tanArg = 2 * pi * plateLength / lambda01;

            var complex = new Complex(0, 1);
            
            return z01 * 
                   (z10 + complex.Imaginary * z01 * Math.Tan(tanArg)) / 
                   (z01 + complex.Imaginary * z10 * Math.Tan(tanArg));
        }

        private double GetZ10(double variantWavelength)
        {
            const double pi = Mathf.PI;
            var a = _internalWaveguideWidth;

            var pow = Math.Pow(variantWavelength / (2 * a), 2);
            var sqrt = Math.Sqrt(1 - pow);
            
            return 120 * pi / sqrt;
        }

        private double GetZ01(double variantWavelength, double plateThickness)
        {
            const double pi = Mathf.PI;
            var a = _internalWaveguideWidth;

            var pow = Math.Pow(variantWavelength / (2 * (a - plateThickness)), 2);
            var sqrt = Math.Sqrt(1 - pow);
            
            return 120 * pi / sqrt;
        }

        private double GetWavelength10(double variantWavelength)
        {
            var a = _internalWaveguideWidth;

            var pow = Math.Pow(variantWavelength / (2 * a), 2);
            var sqrt = Math.Sqrt(1 - pow);
            return variantWavelength / sqrt;
        }

        private double GetWavelength01(double variantWavelength, double plateThickness)
        {
            var a = _internalWaveguideWidth;

            var pow = Math.Pow(variantWavelength / 2 * (a - plateThickness), 2);
            var sqrt = Math.Sqrt(1 - pow);
            return variantWavelength / sqrt;
        }

        private double GetPhaseShift(double plateLength, double wavelength10, double wavelength01)
        {
            const double pi = Mathf.PI;

            return 2 * pi / wavelength10 * plateLength - 2 * pi / wavelength01 * plateLength;
        }

        private double GetVariantWavelength(double frequency)
        {
            return _speedOfLight / frequency;
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