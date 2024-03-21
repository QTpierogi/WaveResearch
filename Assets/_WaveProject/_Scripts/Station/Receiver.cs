using System.Collections;
using TMPro;
using UnityEngine;
using WaveProject.Utils;

namespace WaveProject.Station
{
    public class Receiver : MonoBehaviour
    {
        [Range(0, 5), SerializeField] private float _scaleFactor = 1;
        [SerializeField] private float _speedToTarget = 2;
        [SerializeField] private bool _isEnable;

        [Space]
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Transform _arrow;

        [Space]
        [SerializeField] private ReceivingAntenna _receivingAntenna;
        
        private float _result;
        private int _turnOn;
        private int _speedFactor;

        private float _angleRange = 70;

        private void Start() => StartCoroutine(AimForResultValue());
        
        private float CurrentTarget => _result * _turnOn;

        private void Update()
        {
            _turnOn = _isEnable ? 1 : 0;
            _speedFactor = _isEnable ? 1 : 2;
            
            var power = _receivingAntenna.Power;
            var distanceFactor = _receivingAntenna.GetAntennasDistanceFactor();
            var angleInRadians = GetAngleInRadians(_receivingAntenna.transform.rotation.eulerAngles.z);
            var angleCosine = Mathf.Abs(Mathf.Cos(angleInRadians));
            var value = _scaleFactor * distanceFactor * power * angleCosine;

            var clampedValue = Mathf.Clamp(value, 0, 100);
            _result = clampedValue;
        }

        private IEnumerator AimForResultValue()
        {
            float currentValue = 0;
            while (true)
            {
                currentValue = Mathf.Lerp(currentValue, CurrentTarget, _speedFactor * _speedToTarget * Time.deltaTime);
                
                _text.text = $"{Mathf.Round(currentValue)}";

                var angle = Util.Remap(currentValue, 0, 100, -_angleRange, _angleRange);
                var clampedRotation = Mathf.Clamp(angle, -_angleRange, _angleRange);
                _arrow.rotation = Quaternion.Euler(clampedRotation, 0, 0);
                
                yield return null;
            }
        }
        
        

        private float GetAngleInRadians(float angleInDegree)
        {
            return angleInDegree * Mathf.PI / 180;
        }
    }
}