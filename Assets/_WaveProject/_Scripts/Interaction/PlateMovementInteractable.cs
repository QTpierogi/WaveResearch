using System;
using UnityEngine;
using WaveProject.Station.Plates;
using WaveProject.Utility;

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
                position = _dragZone.bounds.ClosestPoint(hit.point);
                DebugExtension.DebugPoint(position, Color.green);
            }
            else
            {
                var mousePoint = _camera.ScreenToWorldPoint(mousePosition);
                DebugExtension.DebugPoint(mousePoint, Color.red);
                
                position = _dragZone.ClosestPoint(mousePoint);
                DebugExtension.DebugPoint(position, Color.green);
            }

            var distance = Vector3.Distance(position, _target.position);
            var maxDistance = .75f;
            
            if (distance < maxDistance)
            {
                _plate.Rigidbody.position = Vector3.Lerp(_target.position, position, distance / maxDistance);
            }
            else
            {
                _plate.Rigidbody.position = position;
            }

            
            if (Input.GetMouseButtonUp(0))
            {
                _plate.SetKinematic(false);
                FinishChanging();
            }
        }
    }
}