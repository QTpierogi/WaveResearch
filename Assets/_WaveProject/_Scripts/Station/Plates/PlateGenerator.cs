using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WaveProject.Interaction;
using WaveProject.UI;

namespace WaveProject.Station.Plates
{
    public class PlateGenerator : MonoBehaviour
    {
        [SerializeField] private Receiver _receiver;
        [SerializeField] private PlateUiView _plateUiView;

        [Space] 
        [SerializeField] private InteractableButton _selectMetalButton;
        [SerializeField] private InteractableButton _selectDielectricButton;

        [Space] 
        [SerializeField] private int _lengthMinValue;
        [SerializeField] private int _lengthMaxValue;

        [SerializeField] private int _thicknessMinValue;
        [SerializeField] private int _thicknessMaxValue;

        [SerializeField] private int _resistanceMinValue;
        [SerializeField] private int _resistanceMaxValue;

        [Space] 
        [SerializeField] private Plate _metalPlatePrefab;
        [SerializeField] private Plate _dielectricPlatePrefab;
        [SerializeField] private Transform _plateSpawnPoint;

        private float _length;
        private float _thickness;
        private float _resistance;

        private PlateType _plateType;
        private Plate _currentPlate;

        private void Start()
        {
            _plateUiView.Init(Create, 
                _lengthMinValue,
                _lengthMaxValue,
                _thicknessMinValue,
                _thicknessMaxValue,
                _resistanceMinValue,
                _resistanceMaxValue);

            _plateUiView.LengthChanged += OnLengthChanged;
            _plateUiView.ThicknessChanged += OnThicknessChanged;
            _plateUiView.ResistanceChanged += OnResistanceChanged;

            _selectMetalButton.Clicked.AddListener(SelectMetalPlate);
            _selectDielectricButton.Clicked.AddListener(SelectDielectricPlate);
        }

        private void OnDestroy()
        {
            _plateUiView.LengthChanged -= OnLengthChanged;
            _plateUiView.ThicknessChanged -= OnThicknessChanged;
            _plateUiView.ResistanceChanged -= OnResistanceChanged;
        }

        private void OnLengthChanged(float value) => _length = value;
        private void OnThicknessChanged(float value) => _thickness = value;
        private void OnResistanceChanged(float value) => _resistance = value;

        private void SelectMetalPlate()
        {
            _plateType = PlateType.Metal;
            _plateUiView.SelectMetalPlate();
        }

        private void SelectDielectricPlate()
        {
            _plateType = PlateType.Dielectric;
            _plateUiView.SelectDielectricPlate();
        }

        private void Create()
        {
            // if (_length == 0 || _thickness == 0 || _resistance == 0)
            // {
            //     _receiver.SetPhaseShiftPlate(PlateType.None, 0, 0, 0);
            //     return;
            // }

            _receiver.SetPhaseShiftPlate(_plateType, _length, _thickness, _resistance);

            if (_currentPlate != null)
            {
                Destroy(_currentPlate.gameObject);
            }

            _currentPlate = Instantiate(_metalPlatePrefab, _plateSpawnPoint.position, _plateSpawnPoint.rotation,
                _plateSpawnPoint);
            // _currentPlate.SetKinematic(false);
            // _currentPlate.SetSize(_length, _thickness);

            // _plateUiView.Reset();
        }
    }
}