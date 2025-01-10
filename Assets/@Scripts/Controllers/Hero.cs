using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clicker.ContentData;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Scripts;
using UnityEngine;

namespace Clicker.Controllers
{
    public class Hero : Creature
    {
        public Define.HeroMoveState HeroMoveState
        {
            get => _heroMoveState;
            set
            {
                _heroMoveState = value;
            }
        }
        
        [SerializeField] private Define.HeroMoveState _heroMoveState;

        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
            AIProcessAsync().Forget();
            
            if (_moveToCor != null)
            {
                StopCoroutine(_moveToCor);
                _moveToCor = null;
            }

            _cellPosition = Map.WorldToCell(spawnPosition);
            _moveToCor = StartCoroutine(MoveTo());
        }

        protected override void IdleState()
        {
            base.IdleState();
            //0. 사용자가 이동을 하게 될경우에는 강제로 이동하도록 한다
            if (HeroMoveState == Define.HeroMoveState.ForceMove)
            {
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.ForceMove;
                return;
            }

            //1. 근처에 적이 있는지 확인한다.
            BaseObject creature =
                FindNearestCreatureInRange(_searchDistance, Managers.Object.MonsterSet) as Creature;
            if (creature.IsValid())
            {
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.MoveToCreature;
                _targetObject = creature;
                return;
            }
            
            //2. 근처에 env가 있는지 확인한다.
            BaseObject env =
                FindNearestCreatureInRange(_searchDistance, Managers.Object.EnvSet) as Env;
            if (env.IsValid())
            {
                creature =
                    FindNearestCreatureInRange(_searchDistance, Managers.Object.MonsterSet) as Creature;
                if (creature.IsValid())
                {
                    ChangeState(Define.CreatureState.Move);
                    HeroMoveState = Define.HeroMoveState.MoveToCreature;
                    _targetObject = creature;
                    return;
                }

                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.MoveToEnv;
                _targetObject = env;
                return;
            }
            
            //3. heroCamp보다 멀리 떨어져 있으면 이동한다.
            FindPath(Managers.Object.HeroCamp);

            if (_pathQueue.Count > 0)
            {
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.Idle; // 우선 idle 상태에서 moveState에서 State 판단하도록 함
            }
        }
        
        protected override void MoveState()
        {
            base.MoveState();
            
            //0. 사용자가 이동을 하게 될경우에는 강제로 이동하도록 한다
            if (HeroMoveState == Define.HeroMoveState.ForceMove)
            {
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.ForceMove;
                FindPath(Managers.Object.HeroCamp);
                return;
            }
            
            //1. 공격할 대상이 있다면 이동한다.
            if (HeroMoveState == Define.HeroMoveState.MoveToCreature)
            {
                ChaseAndAttack();
                return;
            }

            if (HeroMoveState == Define.HeroMoveState.MoveToEnv)
            {
                ChaseAndAttack();
                return;
            }

            if (HeroMoveState == Define.HeroMoveState.Idle)
            {
                switch (_pathQueue.Count)
                {
                    case > 0:
                        HeroMoveState = Define.HeroMoveState.ReturnToHeroCamp;
                        break;
                    case 0:
                        ChangeState(Define.CreatureState.Idle);
                        break;
                }
            }

            if (HeroMoveState == Define.HeroMoveState.ReturnToHeroCamp)
            {
                //이동할 경로가 더이상 없을 떄
                if (_pathQueue.Count == 0)
                {
                    ChangeState(Define.CreatureState.Idle);
                    HeroMoveState = Define.HeroMoveState.Idle;
                }
            }
        }
        
        protected override void ChaseAndAttack()
        {
            base.ChaseAndAttack();
            
            if (HeroMoveState == Define.HeroMoveState.ForceMove)
            {
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.ForceMove;
                return;
            }
            
            if (!_targetObject.IsValid())
            {
                _targetObject = null;
                ChangeState(Define.CreatureState.Idle);
                HeroMoveState = Define.HeroMoveState.ReturnToHeroCamp;
                return;
            }
            
            float distA = (_targetObject.transform.position - transform.position).sqrMagnitude;
            float distB = AttackDistance * AttackDistance;
            // Debug.Log($"{distA} / {distB}");
            
            //공격 범위안에 들어왔는가
            if (distA <= distB)
            {
                ChangeState(Define.CreatureState.Attack);
                HeroMoveState = Define.HeroMoveState.Idle;
            }
            else
            {
                //추적할 수 있는 거리를 벗어났을 때
                float chaseDistance = _chaseDistance * _chaseDistance;
                if (chaseDistance < distA)
                {
                    HeroMoveState = Define.HeroMoveState.ReturnToHeroCamp;
                    ChangeState(Define.CreatureState.Move);
                    return;
                }
                
                FindPath(_targetObject);
            }
        }

        protected override void AttackState()
        {
            if (!_targetObject.IsValid())
            {
                _targetObject = null;
                ChangeState(Define.CreatureState.Idle);
                HeroMoveState = Define.HeroMoveState.Idle;
                if (_isUseSKill)
                {
                    _isUseSKill = false;
                    _skillBook.StopSkill();
                }
                return;
            }

            Vector3 direction = (_targetObject.transform.position - transform.position).normalized;
            SetFlip(Mathf.Sign(direction.x) == 1);
            
            //기존에 있던 모든 이동 경로는 지운다.
            if (_pathQueue.Count > 0)
            {
                _pathQueue.Clear();
                //_cellPosition = transform.position;
            }
            
            if (_isUseSKill)
            {
                return;
            }
            
            _isUseSKill = true;
            _skillBook.UseSKill(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputHandler.onChangedUIEvent += OnChangedUIEvent;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            InputHandler.onChangedUIEvent -= OnChangedUIEvent;
            if (_animation != null)
            {
                _animation.AnimationState.Event -= OnAnimationEvent;
                _animation.AnimationState.Complete -= OnAnimationComplete;
            }
        }

        private void OnChangedUIEvent(Define.EUIEvent euiEvent)
        {
            switch (euiEvent)
            {
                case Define.EUIEvent.Click:
                    break;
                case Define.EUIEvent.PointerDown:
                    // StopAIProcess();
                    if (_isUseSKill)
                    {
                        _isUseSKill = false;
                        _skillBook.StopSkill();
                    }
                    
                    ChangeState(Define.CreatureState.Move);
                    HeroMoveState = Define.HeroMoveState.ForceMove;
                    break;
                case Define.EUIEvent.PointerUp:
                    ChangeState(Define.CreatureState.Idle);
                    HeroMoveState = Define.HeroMoveState.None;
                    break;
                case Define.EUIEvent.Drag:
                    break;
            }
        }
    }
}