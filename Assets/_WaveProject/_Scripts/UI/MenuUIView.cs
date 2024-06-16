using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WaveProject.Services;
using WaveProject.UserInput;

namespace WaveProject.UI
{
    public class MenuUIView : MonoBehaviour
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _settingsHideMenu;
        [SerializeField] private RectTransform _menu;
        [SerializeField] private Transform _buttonsHolder;
        
        [SerializeField] private float _xOpenedPosition = -60;
        [SerializeField] private float _animationDuration = 1.5f;
        [SerializeField] private Ease _animationEase;

        [SerializeField] private TMP_Dropdown _qualityDropdown;
        [SerializeField] private TMP_Dropdown _fullscreenModeDropdown;

        [SerializeField] private Button _manualButton;
        [SerializeField] private Button _exitButton;

        private InputController _inputController;

        private bool _opened;
        private float _xHidedPosition;
        private float _xHidedButtonsPosition;
        private Sequence _animationSequence;
        private bool _inProcess;
        
        private readonly string _manualPath = Path.Combine(Application.streamingAssetsPath, "manual.pdf");

        private void OnEnable()
        {
            _settingsButton.onClick.AddListener(ToggleShowMenu);
            _settingsHideMenu.onClick.AddListener(ToggleShowMenu);
            
            _qualityDropdown.onValueChanged.AddListener(SetQualityLevel);
            _fullscreenModeDropdown.onValueChanged.AddListener(SetFullscreenMode);

            _manualButton.onClick.AddListener(OpenManual);
            _exitButton.onClick.AddListener(Application.Quit);
        }

        private void OnDisable()
        {
            _settingsButton.onClick.RemoveListener(ToggleShowMenu);
            _settingsHideMenu.onClick.RemoveListener(ToggleShowMenu);
            
            _qualityDropdown.onValueChanged.RemoveListener(SetQualityLevel);
            _fullscreenModeDropdown.onValueChanged.RemoveListener(SetFullscreenMode);

            _manualButton.onClick.RemoveListener(OpenManual);
            _exitButton.onClick.RemoveListener(Application.Quit);
        }

        private void Start()
        {
            ServiceManager.TryGetService(out InputController inputController);
            _inputController = inputController;
            
            _xHidedPosition = _menu.transform.localPosition.x;
            
            var rect = _buttonsHolder.transform.GetChild(0) as RectTransform;;
            _xHidedButtonsPosition = rect.anchoredPosition.x; 
            
            _settingsHideMenu.gameObject.SetActive(_opened);

            LoadQuality();
            LoadFullscreenMode();

            HideExitButtonIfWeb();
        }

        private void HideExitButtonIfWeb()
        {
#if UNITY_WEBGL && UNITY_EDITOR
            _exitButton.gameObject.SetActive(false);
#endif
        }

        private void LoadQuality()
        {
            _qualityDropdown.options = new List<TMP_Dropdown.OptionData>();
            foreach (var qualityLevel in QualitySettings.names)
            {
                _qualityDropdown.options.Add(new TMP_Dropdown.OptionData(qualityLevel));
            }
            
            _qualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        private void LoadFullscreenMode()
        {
            _fullscreenModeDropdown.options = new List<TMP_Dropdown.OptionData> { new("Fullscreen"), new("Windowed") };
            _fullscreenModeDropdown.value = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 0 : 1;
        }

        private void ToggleShowMenu()
        {
            if (_inProcess) return;

            _inProcess = true;
            _opened = !_opened;
            
            _settingsHideMenu.gameObject.SetActive(_opened);
            
            // _inputController.BlockUserInput(_opened);
            // _inputController.BlockMovement(_opened);
            
            _animationSequence?.Kill();
            _animationSequence = DOTween.Sequence();

            int maxCount = _buttonsHolder.childCount;
            int count = 0;

            var animationDuration = GetAnimationDuration(maxCount);
            _animationSequence.Insert(0,
                _menu
                    .DOAnchorPosX(_opened ? _xOpenedPosition : _xHidedPosition, animationDuration)
                    .SetEase(_animationEase));

            foreach (RectTransform buttons in _buttonsHolder.transform)
            {
                count++;
                
                var tween = buttons.DOAnchorPosX(_opened ? 0 : _xHidedButtonsPosition, _opened ? animationDuration : 0)
                    .SetEase(_animationEase);
                
                _animationSequence.Insert(count * animationDuration / maxCount, tween);
            }
            
            _animationSequence.OnKill(() => _inProcess = false);
        }

        private float GetAnimationDuration(int maxCount)
        {
            return _animationDuration / maxCount;
        }

        private void SetQualityLevel(int value)
        {
            QualitySettings.SetQualityLevel(value, true);
        }

        private void SetFullscreenMode(int value)
        {
            var mode = value == 0 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.fullScreenMode = mode;
        }

        private void OpenManual()
        {
            Application.OpenURL(_manualPath);
        }
    }
}