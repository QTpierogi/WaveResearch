using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using WaveProject.Utility;

namespace WaveProject.UserInput
{
    public class CameraDirectionSetter : MonoBehaviour, IInputSubscriber
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private float _fovScaleFactor = .5f;
        
        [SerializeField] private Transform _target;
        [SerializeField] private Collider _moveZone;
        [SerializeField] private float _moveSpeed = 1;
        [SerializeField] private float _fovChangingSpeed = 10;

        private Vector3? _moveTargetPosition;
        
        private float? _defaultFov;
        private float? _fovTarget;
        private float _defaultDistance;

        public event Action ForceUnsubscribed;

        private void Start()
        {
            StartCoroutine(MoveToHitPoint());
            // StartCoroutine(ChangeFovRoutine());
            
            _defaultDistance = Vector3.Distance(_camera.transform.position, _target.position);
            _defaultFov = _camera.m_Lens.FieldOfView;
            _fovTarget = _defaultFov.Value;
        }

        public Transform Transform => transform;

        public void Enable()
        {
            if (_defaultFov != null)
                _fovTarget = _defaultFov;
        }

        public void Disable()
        {
            // StopCoroutine(_moveRoutine);
        }

        public void CustomUpdate(Vector2 _)
        {
            if (!Input.GetMouseButtonDown(0)) return;

            var currentMousePosition = Input.mousePosition;

            var cameraToTargetDistance =
                    Vector3.Distance(Camera.main.transform.position, _target.transform.position);

            var mousePoint = Camera.main.ScreenToWorldPoint(new Vector3(
                currentMousePosition.x,
                currentMousePosition.y,
                cameraToTargetDistance));

            ChangeMoveTargetPosition(mousePoint);
        }

        public void ChangeMoveTargetPosition(Vector3 mousePoint)
        {
            _moveTargetPosition = _moveZone.ClosestPoint(mousePoint);
        }

        public void ChangeFov(Vector3 transformPosition)
        {
            float distance = Vector3.Distance(_camera.transform.position, transformPosition);
            var scale = _defaultDistance / distance;
            _fovTarget *= scale * _fovScaleFactor;
        }

        private IEnumerator MoveToHitPoint()
        {
            yield return new WaitUntil(() => _moveTargetPosition != null);

            while (true)
            {
                _target.position = Vector3.MoveTowards(_target.position, _moveTargetPosition.Value,
                    Easings.OutCubic(Time.deltaTime * _moveSpeed));

                yield return null;
            }
        }

        private IEnumerator ChangeFovRoutine()
        {
            yield return new WaitUntil(() => _fovTarget != null);

            while (true)
            {
                Debug.Log($"Fov target: {_fovTarget.Value}");
                _camera.m_Lens.FieldOfView = Mathf.MoveTowards(_camera.m_Lens.FieldOfView, _fovTarget.Value,
                    Easings.OutCubic(Time.deltaTime * _fovChangingSpeed));

                yield return null;
            }
        }
    }
}