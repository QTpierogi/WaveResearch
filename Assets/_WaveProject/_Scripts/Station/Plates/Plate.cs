using UnityEngine;
using WaveProject.Utility;

namespace WaveProject.Station.Plates
{
    public class Plate : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        
        public void SetKinematic(bool value) => _rigidbody.isKinematic = value;

        public void SetSize(float length, float thickness)
        {
            var lengthInMeters = Utils.MillimetersToMeters(length);
            var thicknessInMeters = Utils.MillimetersToMeters(thickness);
            
            transform.localScale = new Vector3(1, thicknessInMeters, lengthInMeters);
        }
    }
}