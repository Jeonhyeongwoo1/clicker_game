using System;
using System.Threading;
using System.Threading.Tasks;
using Clicker.Entity;
using Clicker.GameComponent;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Scripts;
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
        
        private CreatureMoveComponent _moveComponent;
        [SerializeField] private Define.HeroMoveState _heroMoveState;
        private BaseObject _targetObject;
        private float _distanceThreshold = 5f;
        
        public override bool Init(Define.ObjectType objectType)
        {
            base.Init(objectType);
            //_moveComponent = Util.GetOrAddComponent<CreatureMoveComponent>(gameObject);
            //_moveComponent.Initialize(this, Managers.Game.HeroCamp.transform);
            // _aiComponent = Util.GetOrAddComponent<AIComponent>(gameObject);
            // _aiComponent.Initialize(this);
            
            return true;
        }

        private CancellationTokenSource _aiCts;
        private async UniTaskVoid AIProcess()
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
                    await UniTask.WaitForSeconds(_aiProcessDelay, cancellationToken: _aiCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"{nameof(AIProcess)} / Message : {e.Message}");
                    return;
                }
            }
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
                FindNearestCreatureInRange(5, Managers.Object.MonsterSet) as Creature;
            if (creature.IsValid())
            {
                ChangeState(Define.CreatureState.Move);
                HeroMoveState = Define.HeroMoveState.MoveToCreature;
                _targetObject = creature;
                return;
            }
            
            //2. 근처에 env가 있는지 확인한다.
            BaseObject env =
                FindNearestCreatureInRange(5, Managers.Object.EnvSet) as Env;
            if (env.IsValid())
            {
                creature =
                    FindNearestCreatureInRange(5, Managers.Object.MonsterSet) as Creature;
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
                Vector3 heroCampPos = Managers.Game.HeroCamp.transform.position;
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
            float radius = (_targetObject.Radius + Radius + 1.0f);
            float distB = radius * radius;
            if (distA <= distB)
            {
                direction = Vector2.zero;
                ChangeState(Define.CreatureState.Attack);
                HeroMoveState = Define.HeroMoveState.None;
            }
            else
            {
                float chaseDistance = _searchDistance * _searchDistance;
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
            base.AttackState();

            if (!_targetObject.IsValid())
            {
                _targetObject = null;
                ChangeState(Define.CreatureState.Idle);
                HeroMoveState = Define.HeroMoveState.Idle;
                return;
            }
            
            _targetObject.TakeDamage(this);
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
                case Define.HeroMoveState.Idle:
                    direction = Vector2.zero;
                    SetVelocity(direction, speed);
                    break;
                case Define.HeroMoveState.ForceMove:
                    Vector3 heroCampPos = Managers.Game.HeroCamp.transform.position;
                    Vector2 dir = (heroCampPos - transform.position).normalized;
                    float distToHeroSqrt = (heroCampPos - transform.position).sqrMagnitude;
                    float distToThresholdSqrt = (_distanceThreshold * _distanceThreshold);
                    if (distToHeroSqrt > distToThresholdSqrt)
                    {
                        speed *= 2f;
                    }
                    
                    if (distToHeroSqrt < 1)
                    {
                        direction = Vector2.zero;
                    }
                    else
                    {
                        direction = Vector2.Lerp(_rigidbody2D.velocity.normalized, dir, Time.fixedDeltaTime * 10);
                    }
                    
                    SetVelocity(direction, speed);
                    break;
            }
        }

        private void SetVelocity(Vector2 velocity, float speed)
        {
            _rigidbody2D.velocity = velocity * speed;
            SetFlip(Mathf.Sign(velocity.x) == 1);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputHandler.onChangedUIEvent += OnChangedUIEvent;
            _animation.AnimationState.Event += OnAnimationComplete;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InputHandler.onChangedUIEvent -= OnChangedUIEvent;
            if (_animation != null)
            {
                _animation.AnimationState.Event -= OnAnimationComplete;
            }
        }
        
        private void OnAnimationComplete(TrackEntry trackEntry, Spine.Event e)
        {
            Debug.Log($"animation {trackEntry.Animation.Name}");
        
            // 여기서 원하는 로직 추가
            if (trackEntry.Animation.Name == AnimationName.Attack)
            {
                PlayAnimation(0, AnimationName.Attack, false);
            }
        }

        private void StopAIProcess()
        {
            if (_aiCts != null)
            {
                _aiCts.Cancel();
                _aiCts = null;
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
                    AIProcess().Forget();
                    break;
                case Define.EUIEvent.Drag:
                    break;
            }
        }
    }
}