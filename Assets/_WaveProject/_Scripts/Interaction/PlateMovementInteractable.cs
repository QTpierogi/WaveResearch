using System;
using UnityEngine;
using WaveProject.Station.Plates;

namespace WaveProject.Interaction
{
    internal class PlateMovementInteractable : Interactable
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Plate _plate;
        [SerializeField] private BoxCollider _dragZone;

        [SerializeField] private LayerMask _moveZoneMask;
        
        private Camera _camera;
        private Vector3 _defaultPosition;

        private void Start()
        {
            Init();
            
            _camera = Camera.main;
            _defaultPosition = transform.position;
        }

        public override void CustomUpdate(Vector2 delta)
        {
            _plate.SetKinematic(true);
            
            var cameraToTargetDistance = Vector3.Distance(_camera.transform.position, _plate.Rigidbody.position);
            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraToTargetDistance);
            var ray = _camera.ScreenPointToRay(mousePosition);

            Vector3 position;
            
            if (Physics.Raycast(ray, out var hit, 100, _moveZoneMask))
            {
                Debug.Log("Raycast");
                position = _dragZone.bounds.ClosestPoint(hit.point);
            }
            else
            {
                Debug.Log("mousePoint");

                var mousePoint = _camera.ScreenToWorldPoint(mousePosition);
                position = _dragZone.bounds.ClosestPoint(mousePoint);
            }
            
            _plate.Rigidbody.position = position;
            
            if (Input.GetMouseButtonDown(0))
            {
                _plate.SetKinematic(false);
                FinishChanging();
            }
        }
    }
}