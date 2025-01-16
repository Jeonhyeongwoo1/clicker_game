using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Scripts;
using Scripts.Contents;
using Spine;
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
            StartMoveToCellPosition();
            // _cellPosition = Managers.Map.WorldToCell(_spawnPosition);
        }

        protected override void IdleState()
        {
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
            Define.PathFineResultType resultType = FindPath(Managers.Object.HeroCamp);
            if (resultType == Define.PathFineResultType.Success)
            {
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.ReturnToHeroCamp;
            }
        }

        private void MoveForcePath()
        {
            if (_pathQueue.Count == 0)
            {
                HeroMoveState = Define.HeroMoveState.None;
                return;
            }

            Vector3Int position = _pathQueue.Peek();
            if (CanMoveToCell(position, _cellPosition))
            {
                _pathQueue.Dequeue();
                // Debug.LogWarning("Dequeue :" + _pathQueue.Count);
            }
            else
            {
                BaseObject baseObject = Map.GetBaseObject(position);
                //Hero이고 내가 아니면 종료
                if (baseObject != null && baseObject is Hero && baseObject != this) 
                {
                    HeroMoveState = Define.HeroMoveState.None;
                }
            }
        }


        private bool CheckHeroCampDistance()
        {
            // if (HeroMoveState != Define.HeroMoveState.None)
            // {
            //     return false;
            // }
            
            Vector3 targetPos = Managers.Object.HeroCamp.transform.position;
            if ((targetPos - transform.position).sqrMagnitude < _distanceToTargetThreshold)
            {
                return false;
            }
            
            Vector3Int startPosition = Map.WorldToCell(transform.position);
            Vector3Int destPosition = Map.WorldToCell(targetPos);
            if (!Map.CanGo(destPosition.x, destPosition.y, this))
            {
                return false;
            }
            
            List<Vector3Int> list = Map.PathFinding(startPosition, destPosition, this, 30);
            if (list.Count < 2)
            {
                return false;
            }
            
            _pathQueue = new Queue<Vector3Int>(list);
            _pathQueue.Dequeue();
            HeroMoveState = Define.HeroMoveState.ForcePath;
            return true;
        }
        
        protected override void MoveState()
        {
            if (HeroMoveState == Define.HeroMoveState.ForcePath)
            {
                MoveForcePath();
                return;
            }

            if (CheckHeroCampDistance())
            {
                return;
            }
            
            //0. 사용자가 이동을 하게 될경우에는 강제로 이동하도록 한다
            if (HeroMoveState == Define.HeroMoveState.ForceMove)
            {
                Define.PathFineResultType type = FindPath(Managers.Object.HeroCamp);
                if (type == Define.PathFineResultType.Success)
                {
                }
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
            
                //2. env가 존재하지 않으면 return
                if (!_targetObject.IsValid())
                {
                    ChangeState(Define.CreatureState.Move);
                    HeroMoveState = Define.HeroMoveState.ReturnToHeroCamp;
                    return;
                }
                
                ChaseAndAttack();
                return;
            }
            
            if (HeroMoveState == Define.HeroMoveState.ReturnToHeroCamp)
            {
                Define.PathFineResultType type = FindPath(Managers.Object.HeroCamp);
                if (type == Define.PathFineResultType.Success)
                {
                    HeroMoveState = Define.HeroMoveState.ForcePath;
                    return;
                }

                if (type == Define.PathFineResultType.None)
                {
                    ChangeState(Define.CreatureState.Idle);
                    HeroMoveState = Define.HeroMoveState.Idle;
                    return;
                }
                
                if (type == Define.PathFineResultType.Fail)
                {
                    BaseObject baseObject = Map.GetBaseObject(CellPosition);
                    if (baseObject == this || (baseObject as Hero).CreatureState == Define.CreatureState.Idle)
                    {
                        HeroMoveState = Define.HeroMoveState.Idle;
                        ChangeState(Define.CreatureState.Idle);
                    }
                }
            }
            
            if (HeroMoveState == Define.HeroMoveState.None)
            {
                HeroMoveState = Define.HeroMoveState.ReturnToHeroCamp;
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

            float distA = DistToTargetSqr;
            float distB = AttackDistance * AttackDistance;
            
            // if(ObjectType == Define.EObjectType.Hero)
            // Debug.Log($"dist {distA} / {distB}");
            
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

                
                Define.PathFineResultType resultType = 
                    FindNextPath(_targetObject, 5);
            }
        }

        protected override void AttackState()
        {
            //0. 사용자가 이동을 하게 될경우에는 강제로 이동하도록 한다
            if (HeroMoveState == Define.HeroMoveState.ForceMove)
            {
                ChangeState(Define.CreatureState.Move);
                return;
            }
            
            if (!_targetObject.IsValid())
            {
                _targetObject = null;
                ChangeState(Define.CreatureState.Idle);
                HeroMoveState = Define.HeroMoveState.Idle;
                _skillBook.StopAllSKill();
                return;
            }
            
            base.AttackState();
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
                    _skillBook.StopAllSKill();
                    HeroMoveState = Define.HeroMoveState.ForceMove;
                    break;
                case Define.EUIEvent.PointerUp:
                    ChangeState(Define.CreatureState.Idle);
                    HeroMoveState = Define.HeroMoveState.Idle;
                    break;
                case Define.EUIEvent.Drag:
                    HeroMoveState = Define.HeroMoveState.ForceMove;
                    break;
            }
        }
    }
}