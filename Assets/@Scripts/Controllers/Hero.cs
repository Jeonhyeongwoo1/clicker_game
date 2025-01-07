using System;
using System.Collections.Generic;
using System.Threading;
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
        
        // private CreatureMoveComponent _moveComponent;
        [SerializeField] private Define.HeroMoveState _heroMoveState;
        private float _distanceThreshold = 5f;
        
        public override bool Init(Define.ObjectType objectType)
        {
            base.Init(objectType);
            //_moveComponent = Util.GetOrAddComponent<CreatureMoveComponent>(gameObject);
            //_moveComponent.Initialize(this, Managers.Game.HeroCamp.transform);
            // _aiComponent = Util.GetOrAddComponent<AIComponent>(gameObject);
            // _aiComponent.Initialize(this);

            transform.name = transform.GetInstanceID().ToString();
            return true;
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

            if (HeroMoveState != Define.HeroMoveState.None)
            {
                //3. return to hero camp
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.ReturnToHeroCamp;
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

            //3. 복귀
            if (HeroMoveState == Define.HeroMoveState.ReturnToHeroCamp)
            {
                Vector3 heroCampPos = Managers.Object.HeroCamp.transform.position;
                float distToHeroSqrt = (heroCampPos - transform.position).sqrMagnitude;
                float distToThresholdSqrt = (_distanceThreshold * _distanceThreshold);
                float speed = distToHeroSqrt > distToThresholdSqrt ? MoveSpeed * 1.5f : MoveSpeed;
                Vector2 direction = distToHeroSqrt < 1 ? Vector2.zero : (heroCampPos - transform.position).normalized;
                if (direction == Vector2.zero)
                {
                    ChangeState(Define.CreatureState.Idle);
                    HeroMoveState = Define.HeroMoveState.None;
                }
                
                SetVelocity(direction, speed);
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
                
            Vector3 direction = (_targetObject.transform.position - transform.position).normalized;
            float distA = (transform.position - _targetObject.transform.position).sqrMagnitude;
            float attackDistanceSqrt = AttackDistance;
            float distB = attackDistanceSqrt * attackDistanceSqrt;
            //공격 범위안에 들어왔는가
            if (distA <= distB)
            {
                direction = Vector2.zero;
                ChangeState(Define.CreatureState.Attack);
                HeroMoveState = Define.HeroMoveState.None;
            }
            else
            {
                //추적할 수 있는 거리를 벗어났을 때
                float chaseDistance = _chaseDistance * _chaseDistance;
                if (chaseDistance < distA)
                {
                    HeroMoveState = Define.HeroMoveState.None;
                    ChangeState(Define.CreatureState.Move);
                }
            }
                
            SetVelocity(direction, MoveSpeed);
            return;
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

            if (_isUseSKill)
            {
                return;
            }

            _isUseSKill = true;
            _skillBook.UseSKill(this);
        }

        private Vector3 _heroCampPos;
        private Queue<Vector3Int> _queue;
        private Vector3 _targetPosition;
        private MapManager Map => Managers.Map;

        private void Move()
        {
            Vector2 dir = (_targetPosition - transform.position).normalized;
            float distToHeroSqrt = (_targetPosition - transform.position).sqrMagnitude;
            float distToThresholdSqrt = (_distanceThreshold * _distanceThreshold);
            float speed = MoveSpeed;
            if (distToHeroSqrt > distToThresholdSqrt)
            {
                speed *= 2f;
            }

            Vector3 direction = Vector3.zero;
            if (distToHeroSqrt < 1)
            {
                if (_queue != null && _queue.Count > 0)
                {
                    Vector3Int pos = _queue.Dequeue();
                    _targetPosition = Map.CellToWorld(pos);
                    direction = (_targetPosition - transform.position).normalized;
                }
            }
            else
            {
                direction = Vector2.Lerp(_rigidbody2D.velocity.normalized, dir, Time.fixedDeltaTime * 10);
            }
                    
            SetVelocity(direction, speed);
        }
        
        private void FixedUpdate()
        {
            if (_rigidbody2D == null)
            {
                return;
            }

            Vector2 direction = Vector2.zero;
            float speed = MoveSpeed;
            switch (HeroMoveState)
            {
                case Define.HeroMoveState.None:
                case Define.HeroMoveState.Idle:
                    direction = Vector2.zero;
                    if (_queue != null && _queue.Count > 0)
                    {
                        Move();
                    }
                    else
                    {
                        SetVelocity(direction, speed);
                    }
                    break;
                case Define.HeroMoveState.ForceMove:
                    Vector3 heroCampPos = Managers.Object.HeroCamp.transform.position;
                    if ((heroCampPos - _heroCampPos).sqrMagnitude > 10)
                    {
                        MapManager map = Managers.Map;
                        List<Vector3Int> list = Managers.Map.PathFinding(map.WorldToCell(transform.position),
                            map.WorldToCell(heroCampPos));

                        if (list.Count > 0)
                        {
                            _heroCampPos = heroCampPos;
                            _queue = new Queue<Vector3Int>(list);
                            Vector3Int pos = _queue.Dequeue();
                            _targetPosition = map.CellToWorld(pos);
                        }
                    }

                    Move();
                    break;
            }
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
                    StopAIProcess();
                    ChangeState(Define.CreatureState.Move);
                    HeroMoveState = Define.HeroMoveState.ForceMove;
                    break;
                case Define.EUIEvent.PointerUp:
                    ChangeState(Define.CreatureState.Idle);
                    HeroMoveState = Define.HeroMoveState.None;
                    AIProcessAsync().Forget();
                    break;
                case Define.EUIEvent.Drag:
                    break;
            }
        }
        
        private void OnDrawGizmos()
        {
            if (_queue == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            foreach (Vector3Int vector3Int in _queue)
            {
                Gizmos.DrawSphere(Managers.Map.CellToWorld(vector3Int), 0.3f);
            }
        }
    }
}