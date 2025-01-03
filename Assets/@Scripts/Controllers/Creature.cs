using Clicker.ContentData.Data;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;
using Spine.Unity;

namespace Clicker.Controllers
{
    public class Creature : BaseObject
    {
        public float Speed = 5;

        public Define.CreatureState CreatureState => creatureState;
        [SerializeField] private Define.CreatureState creatureState;
        
        #region Data
        public float AttackCoolTime => _attackCoolTime;
        protected CreatureData _creatureData;
        protected float _attackCoolTime = 1f;
        protected float _maxHp;
        protected float _attack;
        protected float _attackRange;
        protected float _moveSpeed;
        
        #endregion
        
        protected int _currentHp;
        
        protected static class AnimationName
        {
            public static string Idle = "idle";
            public static string Move = "move";
            public static string Attack_a = "attack_a";
            public static string Attack_b = "attack_b";
            public static string Dead = "dead";
        }

        public virtual void SetInfo(CreatureData creatureData)
        {
            _creatureData = creatureData;
            
            _collider2D.offset = new Vector2(_creatureData.ColliderOffsetX, _creatureData.ColliderOffstY);
            _collider2D.radius = _creatureData.ColliderRadius;
            _rigidbody2D.mass = _creatureData.Mass;

            _maxHp = creatureData.MaxHp;
            _attack = creatureData.Atk;
            _attackRange = creatureData.AtkRange;
            _moveSpeed = creatureData.MoveSpeed;
            _attackCoolTime = 0.5f;

            string skeletonDataID = creatureData.SkeletonDataID;
            var dataAsset = Managers.Resource.Load<SkeletonDataAsset>(skeletonDataID);
            _animation.skeletonDataAsset = dataAsset;
            _animation.Initialize(true);
        }
        
        public void ChangeState(Define.CreatureState state)
        {
            if (state == creatureState)
            {
                return;
            }

            creatureState = state;
            switch (state)
            {
                case Define.CreatureState.Idle:
                    ChangeAnimation(AnimationName.Idle);
                    break;
                case Define.CreatureState.Move:
                    ChangeAnimation(AnimationName.Move);
                    break;
                case Define.CreatureState.Attack:
                    ChangeAnimation(AnimationName.Attack_a);
                    break;
                case Define.CreatureState.Dead:
                    ChangeAnimation(AnimationName.Dead);
                    break;
            }
        }

        protected virtual void OnEnable()
        {
        }
 
        protected virtual void OnDisable()
        {
        }

        public virtual void UseSKill()
        {
            float damage = _creatureData.Atk * _creatureData.AtkRange;
            TakeDamage(damage);
        }

        public virtual void TakeDamage(float damage)
        {
            _currentHp -= (int)damage;
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