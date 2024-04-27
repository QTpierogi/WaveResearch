using System;
using UnityEngine;
using WaveProject.Interaction;
using WaveProject.Utility;

namespace WaveProject.Station
{
    public class ReceivingAntenna : MonoBehaviour
    {
        [SerializeField] private float _baseDistance = 4;
        [SerializeField] private MoveInteractable _secondAntenna;
        [SerializeField] private RotateInteractable _rotatePart;
        
        public float Power { get; private set; }
        public float Frequency { get; private set; }

        private void OnValidate()
        {
            _baseDistance = Vector3.Distance(_secondAntenna.Transform.position, transform.position);
        }

        private void Start()
        {
            _secondAntenna.Init();
            _secondAntenna.SetDefaultValue();
            
            _rotatePart.Init();
            _rotatePart.SetDefaultValue(0,0,180);

            var a = Utils.InverseLerp(Vector3.one, Vector3.zero, new Vector3(0.6f,0.6f,0.6f));
            var b = Utils.InverseLerp(Vector3.zero, Vector3.one, new Vector3(0.6f,0.6f,0.6f));
            var c = Utils.InverseLerp(Vector3.one, Vector3.zero, new Vector3(0.25f,0.25f,0.25f));
            var v = Utils.InverseLerp(Vector3.one, Vector3.zero, new Vector3(0.75f,0.75f,0.75f));
        }

        public float GetAntennasDistanceFactor()
        {
          return 1 * _baseDistance / Vector3.Distance(_secondAntenna.Transform.position, transform.position);
        }

        public void SendPower(float power)
        {
            Power = power;
        }

        public void SendFrequency(float frequency)
        {
            Frequency = frequency;
        }
    }
}