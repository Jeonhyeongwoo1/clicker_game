using System.Collections.Generic;
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
        public bool IsDead => CreatureState == Define.CreatureState.Dead;
        public Define.CreatureState CreatureState => _creatureState;
        [SerializeField] protected Define.CreatureState _creatureState;
        
        #region Data
     
        public float MoveSpeed => 5;
        public float AttackCoolTime => _attackCoolTime;
        public float Atk => _atk;
        protected CreatureData _creatureData;
        protected float _attackCoolTime = 1f;
        protected float _atk;
        
        #endregion
        
        protected float _searchDistance = 8f;
        protected float _aiProcessDelay = 0.3f;
        
        protected static class AnimationName
        {
            public static string Idle = "idle";
            public static string Move = "move";
            public static string Attack_a = "attack_a";
            public static string Attack = "attack";
            public static string Attack_b = "attack_b";
            public static string Dead = "dead";
        }

        public override void SetInfo(int id)
        {
            _creatureData = Managers.Data.CreatureDataDict[id];
            _collider2D.offset = new Vector2(_creatureData.ColliderOffsetX, _creatureData.ColliderOffstY);
            _collider2D.radius = _creatureData.ColliderRadius;
            _rigidbody2D.mass = _creatureData.Mass;

            _maxHp = _currentHp = _creatureData.MaxHp;
            _atk = _creatureData.Atk;
            _attackCoolTime = 0.5f;

            string skeletonDataID = _creatureData.SkeletonDataID;
            var dataAsset = Managers.Resource.Load<SkeletonDataAsset>(skeletonDataID);
            _animation.skeletonDataAsset = dataAsset;
            _animation.Initialize(true);
        }
        
        public void ChangeState(Define.CreatureState state)
        {
            if (state == _creatureState)
            {
                return;
            }

            _creatureState = state;
            switch (state)
            {
                case Define.CreatureState.Idle:
                    PlayAnimation(0, AnimationName.Idle, true);
                    break;
                case Define.CreatureState.Move:
                    PlayAnimation(0, AnimationName.Move, true);
                    break;
                case Define.CreatureState.Attack:
                    PlayAnimation(0, AnimationName.Attack, false);
                    break;
                case Define.CreatureState.Dead:
                    PlayAnimation(0, AnimationName.Dead, false);
                    break;
            }
        }

        protected virtual void OnEnable()
        {
        }
 
        protected virtual void OnDisable()
        {
        }

        public override void Dead()
        {
            base.Dead();
            
            ChangeState(Define.CreatureState.Dead);
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
        
        public void SetFlip(bool leftLook)
        {
            _animation.skeleton.ScaleX = leftLook ? -1 : 1;
        }

        protected void ChangeAnimation(string animationName)
        {
            _animation.AnimationName = animationName;
        }

        protected BaseObject FindNearestCreatureInRange(float distance, IEnumerable<BaseObject> objectList)
        {
            float distA = distance * distance;
            BaseObject nearestObj = null;
            float nearestDistance = float.MaxValue;
            foreach (BaseObject obj in objectList)
            {
                float distB = (transform.position - obj.transform.position).sqrMagnitude;
                if (distB < distA && distB < nearestDistance)
                {
                    nearestObj = obj;
                    nearestDistance = distB;
                }
            }

            return nearestObj;
        }

        protected virtual void ChaseAndAttack()
        {}
        protected virtual void IdleState(){ }
        protected virtual void MoveState(){}
        protected virtual void AttackState(){}
        protected virtual void DeadState(){}

    }
}