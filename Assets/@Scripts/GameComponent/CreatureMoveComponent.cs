using Clicker.Controllers;
using UnityEngine;

namespace Clicker.GameComponent
{
    public class CreatureMoveComponent : MonoBehaviour
    {
        private bool _isPossibleMove;
        private Creature _owner;
        private Transform _followTransform;
        private float _speed;
        private Vector3 _prevPosition;
        
        public void Initialize(Creature owner, Transform followTarget)
        {
            _owner = owner;
            _speed = owner.Speed;
            _followTransform = followTarget;
        }

        public void SetIsPossibleMove(bool isPossibleMove)
        {
            _isPossibleMove = isPossibleMove;
        }
        
        private void FixedUpdate()
        {
            Rigidbody2D rigidbody2D = _owner.Rigidbody2D;
            if (!rigidbody2D || !_isPossibleMove)
            {
                return;
            }
            
            Vector2 direction = (_followTransform.position - _owner.transform.position).normalized;
            Vector2 myPos = rigidbody2D.position;
            Vector2 movePosition = Vector2.Lerp(myPos, myPos + direction, Time.fixedDeltaTime * _speed);
            rigidbody2D.MovePosition(movePosition);
            _owner.SetFlip(Mathf.Sign(direction.x) == 1);
           _prevPosition = rigidbody2D.position; 
        }
    }
}