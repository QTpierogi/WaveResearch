using System;
using System.Collections;
using TMPro;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Interaction;
using WaveProject.Utility;
using Random = UnityEngine.Random;

namespace WaveProject
{
    public class CarriageStation : MonoBehaviour
    {
        [SerializeField] private MoveBetweenPointsInteractable _piston;
        [SerializeField] public MoveBetweenPointsInteractable _longCarriage;
        [SerializeField] public MoveBetweenPointsInteractable _crossCarriage;

        [SerializeField] private ReceiverLab1 _receiver;

        [SerializeField] private float ConstDistFromLongToPiston;
        [SerializeField] private float ConstDistFromCrossToPiston;
        [SerializeField] public float ConstStandWidth;

        [SerializeField] private TMP_Text crossOffsetText;
        [SerializeField] private TMP_Text longOffsetText;
        [SerializeField] private TMP_Text pistonOffsetText;


        private float xDistance = 0f;
        private float zDistance = 0f;

        public bool crossInsert = false;
        public bool longInsert = false;
        public bool loopInsert = false;

        public void Init()
        {
            _piston.Init();
            _piston.SetDefaultPosition(0.5f);
            _piston.SetDefaultValue();

            _crossCarriage.Init();
            _crossCarriage.SetDefaultPosition(0.5f); 
            _crossCarriage.SetDefaultValue();

            _longCarriage.Init();
            _longCarriage.SetDefaultPosition(0.5f);
            _longCarriage.SetDefaultValue();
        }

        public void Update()
        {
            crossOffsetText.text = $"{MathF.Round(GetCrossOffset()*1000)}";
            longOffsetText.text = $"{MathF.Round(GetLongOffset()*1000)}";
            pistonOffsetText.text = $"{MathF.Round(GetPistonOffset()*1000)}";

            if (crossInsert)
            {
                xDistance = GetCrossDistanceToPiston();
                zDistance = GetCrossOffset();
            }
            else if (longInsert)
            {
                xDistance = GetLongDistanceToPiston();
                zDistance = 1f;
            }
            else
            {
                xDistance = 0f;
                zDistance = 0f;
            }
            SendData();
        }

        private float GetCrossDistanceToPiston()
        {
            return GetPistonOffset() + ConstDistFromCrossToPiston;
        }

        private float GetLongDistanceToPiston()
        {
            return GetPistonOffset() + GetLongOffset() + ConstDistFromLongToPiston;
        }

        private float GetPistonOffset()
        {
            return Mathf.Abs(_piston.transform.position.z) - Mathf.Abs(_piston._leftPoint.position.z);
        }

        private float GetLongOffset()
        {
            return (Mathf.Abs(_longCarriage._leftPoint.localPosition.z) - Mathf.Abs(_longCarriage.transform.localPosition.z)) /3f;
        }

        private float GetCrossOffset()
        {
            return (Mathf.Abs(_crossCarriage.transform.position.x) - Mathf.Abs(_crossCarriage._leftPoint.position.x)) /5f;
        }

        private void SendData()
        {
            _receiver.SendX(xDistance);
            _receiver.SendZ(zDistance);
        }
    }
}
