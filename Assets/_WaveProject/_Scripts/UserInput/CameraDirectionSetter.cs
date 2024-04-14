using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using WaveProject.Configs;
using WaveProject.Utility;

namespace WaveProject.UserInput
{
    public class CameraDirectionSetter : MonoBehaviour, IInputSubscriber
    {
        [SerializeField] private CinemachineVirtualCamera _camera;

        [SerializeField] private Transform _target;
        [SerializeField] private Collider _moveZone;

        private float _moveSpeed;
        private Vector3? _moveTargetPosition;

        private float? _defaultFov;
        private float? _fovTarget;
        private float _fovScaleFactor;
        private float _fovChangingSpeed;
        private float _defaultDistance;

        public event Action ChangingFinished;

        public Transform Transform => transform;

        private void Start()
        {
            LoadData();

            _defaultDistance = Vector3.Distance(_camera.transform.position, _target.position);
            _defaultFov = _camera.m_Lens.FieldOfView;
            _fovTarget = _defaultFov.Value;

            StartCoroutine(MoveToHitPoint());
            StartCoroutine(ChangeFovRoutine());
        }

        private void LoadData()
        {
            _moveSpeed = InteractionSettings.Data.CameraMoveSpeed;

            _fovScaleFactor = InteractionSettings.Data.FovScaleFactor;
            _fovChangingSpeed = InteractionSettings.Data.FovChangingSpeed;
        }

        public void Enable()
        {
            if (_defaultFov != null)
                _fovTarget = _defaultFov;
        }

        public void Disable()
        {
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
                    Easings.OutCirc(Time.deltaTime * _moveSpeed));

                if (Utils.IsAlmostEqual(_target.position.magnitude, _moveTargetPosition.Value.magnitude, .01))
                {
                    _target.position = _moveTargetPosition.Value;

                    var currentMoveTarget = _moveTargetPosition.Value;
                    yield return new WaitWhile(() => currentMoveTarget == _moveTargetPosition.Value);
                }

                yield return null;
            }
        }

        private IEnumerator ChangeFovRoutine()
        {
            while (true)
            {
                var newFieldOfView = Mathf.Lerp(_camera.m_Lens.FieldOfView, _fovTarget.Value,
                    Easings.OutCirc(Time.deltaTime * _fovChangingSpeed));

                _camera.m_Lens.FieldOfView = newFieldOfView;

                if (Utils.IsAlmostEqual(_camera.m_Lens.FieldOfView, _fovTarget.Value, .01))
                {
                    _camera.m_Lens.FieldOfView = _fovTarget.Value;

                    var currentFovTarget = _fovTarget.Value;
                    yield return new WaitWhile(() => currentFovTarget == _fovTarget.Value);
                }

                yield return null;
            }
        }
    }
}