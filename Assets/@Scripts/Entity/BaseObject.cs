using System;
using System.Runtime.Serialization;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Utils;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Clicker.Entity
{
    public class BaseObject : MonoBehaviour
    {
        public float Radius => _collider2D.radius;
        public Define.EObjectType ObjectType => objectType;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public CircleCollider2D Collider2D => _collider2D;
        public SkeletonAnimation SkeletonAnimation => _animation;
        
        [SerializeField] protected Rigidbody2D _rigidbody2D;
        [SerializeField] protected CircleCollider2D _collider2D;
        [SerializeField] protected SkeletonAnimation _animation;

        protected Define.EObjectType objectType;
        protected float _maxHp;
        protected float _currentHp;
        protected int _id;
        
        private HurtFlashEffect _hurtFlashEffect;
        
        public virtual bool Init(Define.EObjectType eObjectType)
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<CircleCollider2D>();
            _animation = GetComponent<SkeletonAnimation>();
            _hurtFlashEffect = Util.GetOrAddComponent<HurtFlashEffect>(gameObject);
            objectType = eObjectType;
            return true;
        }
        
        protected virtual void OnEnable()
        {
        }
 
        protected virtual void OnDisable()
        {
        }
        public virtual void SetInfo(int id)
        {
            _id = id;
        }

        public virtual void TakeDamage(Creature attacker, SkillData skillData)
        {
            _hurtFlashEffect.Flash();
        }
        
        public virtual void Spawn(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
        }

        public virtual void Dead()
        {
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
                
        protected virtual void OnAnimationComplete(TrackEntry trackEntry)
        {
        }

        protected virtual void OnAnimationEvent(TrackEntry trackEntry, Spine.Event e)
        {
        }

        public virtual float GetTick(Define.CreatureState creatureState)
        {
            switch (creatureState)
            {
                case Define.CreatureState.Idle:
                    return 0.5f;
                case Define.CreatureState.Move:
                    return 0.1f;
                case Define.CreatureState.Attack:
                    return 0.3f;
                case Define.CreatureState.Dead:
                    return 0.1f;
            }

            //default
            return 0.5f;
        }
    }
}