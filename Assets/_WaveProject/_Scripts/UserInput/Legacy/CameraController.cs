using System;
using System.Collections;
using UnityEngine;

namespace WaveProject.UserInput.Legacy
{
    public class CameraController : MonoBehaviour
    {
        [Range(0.01f, 0.5f), SerializeField] private float _partOfScreenToSide = .25f;
        [SerializeField] private float _waitTimeBetweenChangeSide = 1f;

        public ScreenSide CurrentSide { get; private set; }

        public Action<ScreenSide> OnSideChanged;
        private bool _wait;

        private void Update()
        {
            if (_wait) return;
            
            switch (CurrentSide)
            {
                case ScreenSide.Center when IsLeftSide():
                    ChangeSide(ScreenSide.Left);
                    break;
                
                case ScreenSide.Center when IsRightSide():
                    ChangeSide(ScreenSide.Right);
                    break;
                
                case ScreenSide.Left when IsRightSide():
                case ScreenSide.Right when IsLeftSide():
                    ChangeSide(ScreenSide.Center);
                    break;
            }
        }

        private bool IsRightSide() => UnityEngine.Input.mousePosition.x > Screen.width * (1 - _partOfScreenToSide);

        private bool IsLeftSide() => UnityEngine.Input.mousePosition.x < Screen.width * _partOfScreenToSide;

        private void ChangeSide(ScreenSide newSide)
        {
            if (newSide == CurrentSide)
                return;
            
            CurrentSide = newSide;
            OnSideChanged?.Invoke(CurrentSide);

            _wait = true;
            StartCoroutine(Wait(_waitTimeBetweenChangeSide, () => _wait = false));
        }

        private IEnumerator Wait(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}