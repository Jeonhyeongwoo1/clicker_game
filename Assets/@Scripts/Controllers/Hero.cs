using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        [SerializeField] private Define.HeroMoveState _heroMoveState;

        private Vector3 _endMovePosition;
        private readonly int _distanceToTargetThreshold = 10;
        
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
        }

        private IEnumerator MoveTo()
        {
            while (true)
            {
                Vector3 myPos = transform.position;
                while ((myPos - _cellPosition).sqrMagnitude >= 1f)
                {
                    float speed = MoveSpeed;
                    //일정 거리 이상일 경우에는 스피드를 올려준다.
                    if ((myPos - _endMovePosition).sqrMagnitude > _distanceToTargetThreshold * 2)
                    {
                        speed *= 2f;
                    }
                    
                    myPos = transform.position;
                    
                    Vector3 dir = _cellPosition - transform.position;
                    //일관성 있게 이동하기 위해서
                    float moveDist = Mathf.Min(dir.magnitude, speed * Time.deltaTime);
                    transform.position += dir.normalized * moveDist;
                    
                    // Vector3 pos = Vector3.Lerp(myPos, _cellPosition, Time.fixedDeltaTime * speed);
                    // Rigidbody2D.MovePosition(pos);

                    Vector3 direction = (_cellPosition - myPos).normalized;
                    SetFlip(Mathf.Sign(direction.x) == 1);
                    yield return null;
                }

                SetMoveToCellPosition();
                yield return null;
            }
        }
        
        private void SetMoveToCellPosition()
        {
            if (_queue.Count == 0)
            {
                return;
            }
            
            Vector3Int position = _queue.Dequeue();
            bool isPossibleMove = Map.MoveToCell(position, Map.WorldToCell(_cellPosition), this);
            if (isPossibleMove)
            {
                Vector3 cellToWorld = Map.CellToWorld(position);
                _cellPosition = cellToWorld;
            }
            else
            {
                // if (_targetObject == null)
                // {
                //     FindPath(Managers.Object.HeroCamp, true);
                // }
                // else
                // {
                //     FindPath(_targetObject, true);
                // }
                // _queue.Dequeue();
            }
        }

        private void FindPath(BaseObject targetObj, bool forceFindPath = false)
        {
            Vector3 targetPos = targetObj.transform.position;
            if (!forceFindPath && (targetPos - transform.position).sqrMagnitude < _distanceToTargetThreshold)
            {
                return;
            }
            
            Vector3Int startPosition = Map.WorldToCell(transform.position);
            Vector3Int destPosition = Map.GetMoveableTargetPosition(this, targetObj);
            List<Vector3Int> list = Map.PathFinding(startPosition, destPosition);
            if (list.Count <= 2)
            {
                return;
            }
            
            _queue = new Queue<Vector3Int>(list);
            _endMovePosition = Map.CellToWorld(list[^1]);
            Debug.LogWarning("Last : " + _endMovePosition) ;
            if (!forceFindPath)
            {
                SetMoveToCellPosition();
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
                switch (_queue.Count)
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
                if (_queue.Count == 0)
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
            
            float distA = (transform.position - _targetObject.transform.position).sqrMagnitude;
            float attackDistanceSqrt = AttackDistance;
            float distB = attackDistanceSqrt * attackDistanceSqrt;
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

            //기존에 있던 모든 이동 경로는 지운다.
            if (_queue.Count > 0)
            {
                _queue.Clear();
                _cellPosition = transform.position;
            }
            
            Vector3 direction = (_targetObject.transform.position - transform.position).normalized;
            SetFlip(Mathf.Sign(direction.x) == 0);
            if (_isUseSKill)
            {
                return;
            }
            
            _isUseSKill = true;
            _skillBook.UseSKill(this);
        }

        private Vector3 _heroCampPos;
        private Queue<Vector3Int> _queue = new Queue<Vector3Int>();
        private Vector3 _targetPosition;
        private MapManager Map => Managers.Map;
        private Vector3 _cellPosition;
        private Coroutine _moveToCor;

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

        public bool useGizmos = false;
        
        private void OnDrawGizmos()
        {
            if (_queue == null)
            {
                return;
            }

            if (!useGizmos)
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