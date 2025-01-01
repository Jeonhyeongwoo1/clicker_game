using Clicker.Utils;
using UnityEngine;
using Spine.Unity;

namespace Clicker.Controllers
{
    public class Creature : MonoBehaviour
    {
        public float Speed = 5;

        protected float _attackCoolTime = 1f;

        public Define.CreatureType CreatureType => _creatureType;
        protected Define.CreatureType _creatureType;

        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public Collider2D Collider2D => _collider2D;
        
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] protected SkeletonAnimation _animation;

        
        public virtual bool Init(Define.CreatureType creatureType)
        {
            _creatureType = creatureType;
            
            return true;
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void Spawn(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
        }

        public void SetFlip(bool leftLook)
        {
            _animation.skeleton.ScaleX = leftLook ? -1 : 1;
        }

        protected void ChangeAnimation(string animationName)
        {
            _animation.AnimationName = animationName;
        }

    }
}