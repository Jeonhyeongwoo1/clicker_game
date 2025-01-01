using System;
using Clicker.Controllers;
using Scripts;
using UnityEngine;

namespace Clicker.GameComponent
{
    public class CreatureMoveComponent : MonoBehaviour
    {
        private Creature _owner;
        private Vector2 _direction;
        private float _speed;
        
        public void Initialize(Creature owner)
        {
            _owner = owner;
            _speed = owner.Speed;
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

        private void FixedUpdate()
        {
            Rigidbody2D rigidbody2D = _owner.Rigidbody2D;
            if (!rigidbody2D)
            {
                return;
            }
            
            Vector2 myPos = rigidbody2D.position;
            Vector2 movePosition = Vector2.Lerp(myPos, myPos + _direction, Time.fixedDeltaTime * _speed);
            rigidbody2D.MovePosition(movePosition);
            _owner.SetFlip(Mathf.Sign(_direction.x) == 1);
            
            //double angle = Math.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg - 90f;
            //Debug.Log(angle);
        }
    }
}