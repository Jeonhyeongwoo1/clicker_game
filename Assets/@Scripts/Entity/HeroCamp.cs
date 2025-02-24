using System;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Utils;
using Scripts;
using UnityEngine;

namespace Clicker.Entity
{
    public class HeroCamp : BaseObject
    {
        public Vector3 MovePosition => _movePosition; 
        
        private CameraController _cameraController;
        private Vector2 _direction;
        private Vector3 _movePosition;
        private float _speed = 5f;

        public override bool Init(Define.EObjectType eObjectType)
        {
            if(base.Init(eObjectType) == false)
                return false;

            _collider2D.excludeLayers = (1 << (int)Define.ELayer.Monster) | (1 << (int)Define.ELayer.Hero);
            _collider2D.includeLayers = (1 << (int)Define.ELayer.Obstacle);
            
            if (_cameraController == null)
            {
                _cameraController = FindObjectOfType<CameraController>();
            }

            _cameraController.SetTarget(transform);
            return true;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        protected override void OnEnable()
        {
            InputHandler.onDragAction += SetDirection;
            InputHandler.onPointerUpAction += SetDirection;
        }

        protected override void OnDisable()
        {
            InputHandler.onDragAction -= SetDirection;
            InputHandler.onPointerUpAction -= SetDirection;
        }

        public void MoveToWaypointPosition(Vector3 waypointPosition)
        {
            transform.position = waypointPosition;
            // _cameraController.MoveToWayPoint(waypointPosition);
        }

        private void SetDirection(Vector2 position)
        {
            _direction = position;
        }
   
        private void Update()
        {
            Vector2 myPos = transform.position;
            Vector2 movePosition = Vector2.Lerp(myPos, myPos + _direction, Time.deltaTime * _speed);
            if (Managers.Map.IsPossibleHeroCampMove(movePosition))
            {
                transform.position = movePosition;
                _movePosition = movePosition;

                bool isInStage = Managers.Map.StageTranslation.IsInStageInRange(movePosition);
                if (!isInStage)
                {
                    Managers.Map.StageTranslation.ChangeStage(_movePosition);
                }
            }
            
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}