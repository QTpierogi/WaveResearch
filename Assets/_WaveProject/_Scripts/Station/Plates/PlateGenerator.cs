using UnityEngine;
using WaveProject.Interaction;

namespace WaveProject.Station.Plates
{
    public class PlateGenerator : MonoBehaviour
    {
        [SerializeField] private Receiver _receiver;

        [SerializeField] private Toggle _powerToggle;

        [Space] [SerializeField] private InteractableButton _selectMetalButton;
        [SerializeField] private InteractableButton _selectDielectricButton;

        [Space] [SerializeField] private GameObject _selectView;
        [SerializeField] private GameObject _setupView;
        [SerializeField] private GameObject _resetView;

        [Space] [SerializeField] private InteractableButton _increaseLengthButton;
        [SerializeField] private InteractableButton _decreaseLengthButton;

        [Space] [SerializeField] private InteractableButton _increaseThicknessButton;
        [SerializeField] private InteractableButton _decreaseThicknessButton;

        [Space] [SerializeField] private InteractableButton _increaseResistanceButton;
        [SerializeField] private InteractableButton _decreaseResistanceButton;

        [Space] [SerializeField] private InteractableButton _backButton;
        [SerializeField] private InteractableButton _createButton;

        [Space] [SerializeField] private Rigidbody _platePrefab;

        private float _length;
        private float _thickness;
        private float _resistance;
        private PlateType _plateType;

        private void Start()
        {
            TogglePower(false);
            _powerToggle.Toggled.AddListener(TogglePower);
            _selectMetalButton.Clicked.AddListener(SelectMetalPlate);
            _selectDielectricButton.Clicked.AddListener(SelectDielectricPlate);

            _increaseLengthButton.Clicked.AddListener(IncreaseLength);
            _increaseThicknessButton.Clicked.AddListener(IncreaseThickness);
            _increaseResistanceButton.Clicked.AddListener(IncreaseResistance);

            _decreaseLengthButton.Clicked.AddListener(DecreaseLength);
            _decreaseThicknessButton.Clicked.AddListener(DecreaseThickness);
            _decreaseResistanceButton.Clicked.AddListener(DecreaseResistance);

            _backButton.Clicked.AddListener(Reset);

            _createButton.Clicked.AddListener(Create);
        }

        private void Reset() => TogglePower(true);

        private void ChangeParameter(ref float parameter, float delta, float min, float max)
        {
            parameter += delta;
            parameter = Mathf.Clamp(parameter, min, max);
        }

        private void IncreaseLength() => ChangeParameter(ref _length, +0.1f, 0, 100);
        private void DecreaseLength() => ChangeParameter(ref _length, -0.1f, 0, 100);
        private void IncreaseThickness() => ChangeParameter(ref _thickness, +0.1f, 0, 100);
        private void DecreaseThickness() => ChangeParameter(ref _thickness, -0.1f, 0, 100);
        private void IncreaseResistance() => ChangeParameter(ref _resistance, +0.1f, 0, 100);
        private void DecreaseResistance() => ChangeParameter(ref _resistance, -0.1f, 0, 100);

        private void SelectMetalPlate()
        {
            _plateType = PlateType.Metal;
            _setupView.SetActive(true);
            _selectView.SetActive(false);
            _resetView.SetActive(true);

            _increaseResistanceButton.gameObject.SetActive(false);
            _decreaseResistanceButton.gameObject.SetActive(false);
        }

        private void SelectDielectricPlate()
        {
            _plateType = PlateType.Dielectric;
            _setupView.SetActive(true);
            _selectView.SetActive(false);
            _resetView.SetActive(true);

            _increaseResistanceButton.gameObject.SetActive(true);
            _decreaseResistanceButton.gameObject.SetActive(true);
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
            _receiver.SetPhaseShiftPlate(_plateType, _length, _thickness, _resistance);

            _platePrefab.gameObject.SetActive(false);
            
            var newPlate = Instantiate(_platePrefab, _platePrefab.position, _platePrefab.rotation, transform);
            newPlate.isKinematic = false;
            newPlate.gameObject.SetActive(true);
            newPlate.transform.localScale = new Vector3(1, _thickness, _length);
                
            Reset();
        }
    }
}