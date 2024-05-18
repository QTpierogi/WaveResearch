using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WaveProject.UI
{
    public class PlateUiView : MonoBehaviour
    {
        [Space] 
        [SerializeField] private Slider _lengthSlider;
        [SerializeField] private Slider _thicknessSlider;
        [SerializeField] private Slider _resistanceSlider;

        [Space] 
        [SerializeField] private TMP_Text _lengthText;
        [SerializeField] private TMP_Text _thicknessText;
        [SerializeField] private TMP_Text _resistanceText;
        
        [Space] 
        [SerializeField] private GameObject _resistance;
        
        [Space] 
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _createButton;
        private bool _lengthConfigured;
        private bool _thicknessConfigured;
        private bool _resistanceConfigured;

        public event Action<float> LengthChanged;
        public event Action<float> ThicknessChanged;
        public event Action<float> ResistanceChanged;

        public void Init(Action create, Action back,
            int lengthMinValue,
            int lengthMaxValue,
            int thicknessMinValue,
            int thicknessMaxValue,
            float resistanceMinValue,
            float resistanceMaxValue)
        {
            _lengthSlider.minValue = lengthMinValue;
            _lengthSlider.maxValue = lengthMaxValue;
            _lengthSlider.value = 0;

            _thicknessSlider.minValue = thicknessMinValue;
            _thicknessSlider.maxValue = thicknessMaxValue;
            _thicknessSlider.value = 0;

            _resistanceSlider.minValue = resistanceMinValue;
            _resistanceSlider.maxValue = resistanceMaxValue;
            _resistanceSlider.value = 0;

            _lengthSlider.onValueChanged.AddListener(ChangeLength);
            _thicknessSlider.onValueChanged.AddListener(ChangeThickness);
            _resistanceSlider.onValueChanged.AddListener(ChangeResistance);

            _backButton.onClick.AddListener(() => back());
            _createButton.onClick.AddListener(() => create());
            
            _createButton.interactable = false;
        }

        private void ChangeLength(float value)
        {
            _lengthConfigured = true;
            LengthChanged?.Invoke(ChangeParameter(_lengthText, value));
        }

        private void ChangeThickness(float time)
        {
            _thicknessConfigured = true;
            ThicknessChanged?.Invoke(ChangeParameter(_thicknessText, time));
        }

        private void ChangeResistance(float time)
        {
            _resistanceConfigured = true;
            ResistanceChanged?.Invoke(ChangeParameter(_resistanceText, time));
        }

        private static float ChangeParameter(TMP_Text text, float value)
        {
            text.text = value.ToString("F");
            return value;
        }

        public void SelectMetalPlate()
        {
            _resistance.gameObject.SetActive(false);
            _createButton.interactable = false;

            StartCoroutine(WaitAllConfigured(true));
        }

        public void SelectDielectricPlate()
        {
            _resistance.gameObject.SetActive(true);
            _createButton.interactable = false;

            StartCoroutine(WaitAllConfigured(false));
        }

        private IEnumerator WaitAllConfigured(bool isMetal)
        {
            if (isMetal)
            {
                yield return new WaitWhile(() => _lengthConfigured == false ||
                                                 _thicknessConfigured == false);
            }
            else
            {
                yield return new WaitWhile(() => _lengthConfigured == false ||
                                                 _thicknessConfigured == false ||
                                                 _resistanceConfigured == false);
            }
            
            _createButton.interactable = true;
        }
    }
}