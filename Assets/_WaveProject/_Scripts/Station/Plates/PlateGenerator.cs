using System.Globalization;
using TMPro;
using UnityEngine;
using WaveProject.Interaction;

namespace WaveProject.Station.Plates
{
    public class PlateGenerator : MonoBehaviour
    {
        [SerializeField] private Receiver _receiver;

        [SerializeField] private Toggle _powerToggle;

        [Space] 
        [SerializeField] private InteractableButton _selectMetalButton;
        [SerializeField] private InteractableButton _selectDielectricButton;

        [Space] 
        [SerializeField] private GameObject _selectView;
        [SerializeField] private GameObject _setupView;
        [SerializeField] private GameObject _resetView;

        [SerializeField] private Slider3D _lengthSlider;
        [SerializeField] private Slider3D _thicknessSlider;
        [SerializeField] private Slider3D _resistanceSlider;
        
        [Space] 
        [SerializeField] private TMP_Text _lengthText;
        [SerializeField] private TMP_Text _thicknessText;
        [SerializeField] private TMP_Text _resistanceText;
        
        [Space] 
        [SerializeField] private InteractableButton _backButton;
        [SerializeField] private InteractableButton _createButton;

        [Space] 
        [SerializeField] private Plate _platePrefab;
        [SerializeField] private Transform _plateSpawnPoint;

        private float _length;
        private float _thickness;
        private float _resistance;
        private PlateType _plateType;
        
        private Plate _currentPlate;

        private void Start()
        {
            TogglePower(false);
            _powerToggle.Toggled.AddListener(TogglePower);
            _selectMetalButton.Clicked.AddListener(SelectMetalPlate);
            _selectDielectricButton.Clicked.AddListener(SelectDielectricPlate);
            
            _lengthSlider.Init();
            _thicknessSlider.Init();
            _resistanceSlider.Init();

            _lengthSlider.OnValueChanged.AddListener(ChangeLength);
            _thicknessSlider.OnValueChanged.AddListener(ChangeThickness);
            _resistanceSlider.OnValueChanged.AddListener(ChangeResistance);

            _backButton.Clicked.AddListener(Reset);

            _createButton.Clicked.AddListener(Create);
        }

        private void Reset() => TogglePower(true);

        private void ChangeLength(float time) => _length = ChangeParameter(_lengthText, time, 0, 100);
        private void ChangeThickness(float time) => _thickness = ChangeParameter(_thicknessText, time, 0, 100);
        private void ChangeResistance(float time) => _resistance = ChangeParameter(_resistanceText, time, 0, 100);

        private float ChangeParameter(TMP_Text lengthText, float time, float min, float max)
        {
            var newValue = Mathf.Lerp(min, max, time);
            lengthText.text = newValue.ToString(CultureInfo.InvariantCulture);
            return newValue;
        }

        private void SelectMetalPlate()
        {
            _plateType = PlateType.Metal;
            _setupView.SetActive(true);
            _selectView.SetActive(false);
            _resetView.SetActive(true);

            _resistanceText.gameObject.SetActive(false);
            _resistanceSlider.Hide();
        }

        private void SelectDielectricPlate()
        {
            _plateType = PlateType.Dielectric;
            _setupView.SetActive(true);
            _selectView.SetActive(false);
            _resetView.SetActive(true);

            _resistanceText.gameObject.SetActive(true);
            _resistanceSlider.Show();
        }

        private void TogglePower(bool isOn)
        {
            if (isOn)
            {
                _selectView.SetActive(true);
                _setupView.SetActive(false);
                _resetView.SetActive(false);
            }
            else
            {
                _selectView.SetActive(false);
                _setupView.SetActive(false);
                _resetView.SetActive(false);
            }
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
            
            _currentPlate = Instantiate(_platePrefab, _plateSpawnPoint.position, _plateSpawnPoint.rotation, _plateSpawnPoint);
            _currentPlate.SetKinematic(false);
            _currentPlate.gameObject.SetActive(true);
            _currentPlate.SetSize(_length, _thickness);
                
            Reset();
        }
    }
}