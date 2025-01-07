using System;
using System.Collections.Generic;
using System.Threading;
using Clicker.ContentData;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Skill;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Spine;
using UnityEngine;
using Spine.Unity;
using Event = Spine.Event;

namespace Clicker.Controllers
{
    public class Creature : BaseObject
    {
        public bool IsDead => CreatureState == Define.CreatureState.Dead;
        public Define.CreatureState CreatureState => _creatureState;
        public BaseObject TargetObject => _targetObject;
        
        [SerializeField] protected Define.CreatureState _creatureState;
        
        #region Data
     
        public float MoveSpeed => _moveSpeed;
        public float Atk => _atk;
        public float AttackRange => _attackRange;
        protected CreatureData _creatureData;
        protected float _atk;
        protected float _attackRange;
        protected float _moveSpeed;
        
        #endregion

        protected float AttackDistance
        {
            get
            {
                //최소범위
                float radius = (_targetObject.Radius + Radius + 1.0f);
                return radius + _attackRange;
            }
        }
        
        protected SkillBook _skillBook;
        protected float _chaseDistance = 8f;
        protected float _searchDistance = 8f;
        protected CancellationTokenSource _aiCts;
        protected BaseObject _targetObject;
        protected bool _isUseSKill = false;
        
        public override void SetInfo(int id)
        {
            base.SetInfo(id);
            switch(ObjectType)
            {
                case Define.EObjectType.Hero:
                    _creatureData = Managers.Data.HeroDataDict[id];
                    break;
                case Define.EObjectType.Monster:
                    _creatureData = Managers.Data.MonsterDataDict[id];
                    break;
            }
            
            _collider2D.offset = new Vector2(_creatureData.ColliderOffsetX, _creatureData.ColliderOffstY);
            _collider2D.radius = _creatureData.ColliderRadius;
            _rigidbody2D.mass = _creatureData.Mass;

            _maxHp = _currentHp = _creatureData.MaxHp;
            _atk = _creatureData.Atk;
            _attackRange = _creatureData.AtkRange;
            _moveSpeed = _creatureData.MoveSpeed;

            string skeletonDataID = _creatureData.SkeletonDataID;
            var dataAsset = Managers.Resource.Load<SkeletonDataAsset>(skeletonDataID);
            _animation.skeletonDataAsset = dataAsset;
            _animation.Initialize(true);

            _skillBook = Util.GetOrAddComponent<SkillBook>(gameObject);
            _skillBook.AddSkill(_creatureData.SkillIdList);

            _animation.AnimationState.Event += OnAnimationEvent;
            _animation.AnimationState.Complete += OnAnimationComplete;
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
                    PlayAnimation(0, Define.AnimationName.Idle, true);
                    break;
                case Define.CreatureState.Move:
                    PlayAnimation(0, Define.AnimationName.Move, true);
                    break;
                case Define.CreatureState.Attack:
                    if (ObjectType == Define.EObjectType.Hero)
                    {
                        PlayAnimation(0, Define.AnimationName.Attack, false);
                    }
                    else
                    {
                        PlayAnimation(0, Define.AnimationName.Attack_a, false);
                    }
                    break;
                case Define.CreatureState.Dead:
                    PlayAnimation(0, Define.AnimationName.Dead, false);
                    break;
            }
        }

        public override void Dead()
        {
            ChangeState(Define.CreatureState.Dead);
            _skillBook.StopSkill();
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

        protected override void OnAnimationComplete(TrackEntry trackEntry)
        {
            base.OnAnimationComplete(trackEntry);

            if (trackEntry.Animation.Name == Define.AnimationName.Attack &&
                _creatureState == Define.CreatureState.Attack)
            {
                PlayAnimation(0, Define.AnimationName.Attack, false);
            }
            else if (trackEntry.Animation.Name == Define.AnimationName.Attack_a &&
                     _creatureState == Define.CreatureState.Attack)
            {
                PlayAnimation(0, Define.AnimationName.Attack_a, false);
            }
            else if (trackEntry.Animation.Name == Define.AnimationName.Attack_b &&
                     _creatureState == Define.CreatureState.Attack)
            {
                PlayAnimation(0, Define.AnimationName.Attack_b, false);
            }
            else if (trackEntry.Animation.Name == Define.AnimationName.Dead &&
                     _creatureState == Define.CreatureState.Dead)
            {
                Dead();
            }
        }

        protected async UniTaskVoid AIProcessAsync()
        {
            _aiCts = new CancellationTokenSource();
            while (_aiCts.IsCancellationRequested == false)
            {
                switch (_creatureState)
                {
                    case Define.CreatureState.Idle:
                        IdleState();
                        break;
                    case Define.CreatureState.Move:
                        MoveState();
                        break;
                    case Define.CreatureState.Attack:
                        AttackState();
                        break;
                    case Define.CreatureState.Dead:
                        DeadState();
                        break;
                }

                try
                {
                    await UniTask.WaitForSeconds(GetTick(_creatureState), cancellationToken: _aiCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"{nameof(AIProcessAsync)} / Message : {e.Message}");
                    return;
                }
            }
        }

        protected void StopAIProcess()
        {
            if (_aiCts != null)
            {
                _aiCts.Cancel();
                _aiCts = null;
            }
        }

        protected void SetVelocity(Vector2 velocity, float speed)
        {
            _rigidbody2D.velocity = velocity * speed;
            SetFlip(Mathf.Sign(velocity.x) == 1);
        }

        protected virtual void ChaseAndAttack() {}
        protected virtual void IdleState() {}
        protected virtual void MoveState(){}
        protected virtual void AttackState(){}

        protected virtual void DeadState()
        {
            ChangeState(Define.CreatureState.Dead);
        }

    }
}