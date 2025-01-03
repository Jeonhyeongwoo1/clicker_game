using System.Runtime.Serialization;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Utils;
using Spine.Unity;
using UnityEngine;

namespace Clicker.Entity
{
    public class BaseObject : MonoBehaviour
    {
        public float Radius => _collider2D.radius;
        public Define.ObjectType ObjectType => _objectType;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public CircleCollider2D Collider2D => _collider2D;
        
        [SerializeField] protected Rigidbody2D _rigidbody2D;
        [SerializeField] protected CircleCollider2D _collider2D;
        [SerializeField] protected SkeletonAnimation _animation;

        protected Define.ObjectType _objectType;
        protected float _maxHp;
        protected float _currentHp;
        protected int _id;
        
        public virtual bool Init(Define.ObjectType objectType)
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<CircleCollider2D>();
            _animation = GetComponent<SkeletonAnimation>();
            _objectType = objectType;
            return true;
        }

        public virtual void SetInfo(int id)
        {
            _id = id;
        }

        public virtual void TakeDamage(Creature attacker)
        {
            _currentHp -= (int) Mathf.Clamp(attacker.Atk, 0, attacker.Atk);

            Debug.Log($"{_currentHp} / {attacker.Atk}");
            if (_currentHp <= 0)
            {
                Dead();    
            }
        }
        
        public virtual void Spawn(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
        }

        public virtual void Dead()
        {
            Managers.Object.Despawn(this);
        }

        protected virtual void UpdateAnimation()
        {
        }
        
        public void PlayAnimation(int trackIndex, string AnimName, bool loop)
        {
            if (_animation == null)
                return;

            _animation.AnimationState.SetAnimation(trackIndex, AnimName, loop);
        }
    }
}