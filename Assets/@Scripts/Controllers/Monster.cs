using Clicker.Entity;
using Clicker.GameComponent;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Controllers
{
    public class Monster : Creature
    {
        private Vector3 _spawnPosition;
        
        public override bool Init(Define.EObjectType eObjectType)
        {
            base.Init(eObjectType);
            
            ChangeState(Define.CreatureState.Idle);
            return true;
        }

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
            _spawnPosition = _cellPosition;
            _moveToCor = StartCoroutine(MoveTo());
        }

        protected override void AttackState()
        {
            base.AttackState();
            
            //타겟이 죽었는가?
            if (!_targetObject.IsValid())
            {
                _targetObject = null;
                ChangeState(Define.CreatureState.Idle);
                if (_isUseSKill)
                {
                    _isUseSKill = false;
                    _skillBook.StopSkill();
                }
                return;
            }

            Vector3 direction = (_targetObject.transform.position - transform.position).normalized;
            SetFlip(Mathf.Sign(direction.x) == 1);
            
            //공격중에 타겟이 사정거리 밖으로 이동한다면 추적
            float distA = (transform.position - _targetObject.transform.position).sqrMagnitude;
            float distB = AttackDistance * AttackDistance;
            //공격 범위안에 들어왔는가
            if (distA <= distB)
            {
                if (_isUseSKill)
                {
                    return;
                }

                _isUseSKill = true;
                _skillBook.UseSKill(this);
            }
            else
            {
                
                //추적할 수 있는 거리를 벗어났을 때
                float chaseDistance = _chaseDistance * _chaseDistance;
                if (chaseDistance < distA)
                {
                    ChangeState(Define.CreatureState.Idle);
                }
                else
                {
                    if (_isUseSKill)
                    {
                        _isUseSKill = false;
                        _skillBook.StopSkill();
                    }
                    
                    ChangeState(Define.CreatureState.Move);
                    FindPath(_targetObject);
                }
            }
        }

        protected override void IdleState()
        {
            base.IdleState();
            
            //1. 근처에 적이 있는지 확인한다.
            BaseObject creature =
                FindNearestCreatureInRange(_searchDistance, Managers.Object.HeroSet) as Creature;
            if (creature.IsValid())
            {
                ChangeState(Define.CreatureState.Move);
                _targetObject = creature;
                return;
            }

            Rigidbody2D.velocity = Vector2.zero;
        }

        protected override void MoveState()
        {
            base.MoveState();

            //타켓 유무확인
            if (_targetObject.IsValid())
            {
                ChaseAndAttack();
                return;
            }

            //타켓이 없으면 다시 원래 위치로 이동
            MoveToSpawnPosition();
            return;
        }

        private void MoveToSpawnPosition()
        {
            float distToSpawnSqrt = (transform.position - _spawnPosition).sqrMagnitude;
            if (distToSpawnSqrt < 3f)
            {
                ChangeState(Define.CreatureState.Idle);
                return;
            }

            FindPath(_spawnPosition);
        }

        protected override void ChaseAndAttack()
        {
            if (!_targetObject.IsValid())
            {
                _targetObject = null;
                ChangeState(Define.CreatureState.Idle);
                return;
            }
            
            float distA = (transform.position - _targetObject.transform.position).sqrMagnitude;
            float distB = AttackDistance * AttackDistance;
            
            //공격 범위안에 들어왔는가
            if (distA <= distB)
            {
                ChangeState(Define.CreatureState.Attack);
            }
            else
            {
                ChangeState(Define.CreatureState.Move);
                
                //추적할 수 있는 거리를 벗어났을 때
                float chaseDistance = _chaseDistance * _chaseDistance;
                if (chaseDistance < distA)
                {
                    _targetObject = null;
                    return;
                }
                
                FindPath(_targetObject);
            }
        }
    }
}