using System;
using System.Collections;
using UnityEngine;

namespace WaveProject.UserInput
{
    public class CameraDirectionSetter : MonoBehaviour, IInputSubscriber
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Collider _moveZone;
        [SerializeField] private float _moveSpeed = 1;

        private Vector3? _moveTargetPosition;
        private Coroutine _moveRoutine;

        public event Action ForceUnsubscribe;

        public void Enable()
        {
            _moveRoutine = StartCoroutine(MoveToHitPoint());
        }

        public void Disable()
        {
            StopCoroutine(_moveRoutine);
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

            _moveTargetPosition = _moveZone.ClosestPoint(mousePoint);
        }

        private IEnumerator MoveToHitPoint()
        {
            yield return new WaitUntil(() => _moveTargetPosition != null);

            while (true)
            {
                _target.position = Vector3.MoveTowards(_target.position, _moveTargetPosition.Value,
                    Mathf.Sqrt(1 - Mathf.Pow(Time.deltaTime * _moveSpeed - 1, 2)));

                yield return null;
            }
        }
    }
}