using System;
using Clicker.Controllers;
using Clicker.Utils;
using Scripts;
using UnityEngine;

namespace Clicker.Entity
{
    public class HeroCamp : BaseObject
    {
        private Vector2 _direction;
        private float _speed = 5f;

        public override bool Init(Define.ObjectType objectType)
        {
            if(base.Init(objectType) == false)
                return false;

            _collider2D.excludeLayers = (1 << (int)Define.ELayer.Monster) | (1 << (int)Define.ELayer.Hero);
            _collider2D.includeLayers = (1 << (int)Define.ELayer.Obstacle);
            
            return true;
        }


        private void OnEnable()
        {
            InputHandler.onDragAction += SetDirection;
            InputHandler.onPointerUpAction += SetDirection;
        }

        private void OnDisable()
        {
            InputHandler.onDragAction -= SetDirection;
            InputHandler.onPointerUpAction -= SetDirection;
        }

        private void SetDirection(Vector2 position)
        {
            _direction = position;
        }
   
        private void Update()
        {
            Vector2 myPos = transform.position;
            Vector2 movePosition = Vector2.Lerp(myPos, myPos + _direction, Time.deltaTime * _speed);
            transform.position = movePosition;
            
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}