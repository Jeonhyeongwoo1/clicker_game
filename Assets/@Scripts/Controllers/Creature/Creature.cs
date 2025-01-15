using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Clicker.ContentData;
using Clicker.Effect;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Skill;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Spine;
using UnityEngine;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Clicker.Controllers
{
    public class Creature : BaseObject
    {
        public CreatureData CreatureData => _creatureData;
        public bool IsDead => CreatureState == Define.CreatureState.Dead;
        public Define.CreatureState CreatureState => _creatureState;
        public BaseObject TargetObject => _targetObject;
        public EffectComponent EffectComponent => _effectComponent;
        
        protected MapManager Map => Managers.Map;

        #region CreatureStat
        public CreatureStat MoveSpeed => _moveSpeed;
        public CreatureStat Atk => _atk;
        public CreatureStat AtkBonus => _atkBonus;
        public CreatureStat CriRate => _criRate;
        public CreatureStat CriDamage => _criDamage;
        public CreatureStat AttackRange => _attackRange;
        public CreatureStat ReduceDamageRate => _reduceDamageRate;
        public CreatureStat ThornsDamageRate => _thornsDamageRate;
        public CreatureStat AttackSpeedRate => _attackSpeedRate;
        public CreatureStat LifeStealRate => _lifeStealRate;
        
        [SerializeField][ReadOnly] protected CreatureStat _atk;
        [SerializeField][ReadOnly] protected CreatureStat _attackRange;
        [SerializeField][ReadOnly] protected CreatureStat _moveSpeed;
        [SerializeField][ReadOnly] protected CreatureStat _atkBonus;
        [SerializeField][ReadOnly] protected CreatureStat _criRate;
        [SerializeField][ReadOnly] protected CreatureStat _criDamage;
        [SerializeField][ReadOnly] protected CreatureStat _maxHp;
        [SerializeField][ReadOnly] protected CreatureStat _reduceDamageRate;
        [SerializeField][ReadOnly] protected CreatureStat _thornsDamageRate;
        [SerializeField][ReadOnly] protected CreatureStat _attackSpeedRate;
        [SerializeField][ReadOnly] protected CreatureStat _lifeStealRate;

        protected float _currentHp;
        protected CreatureData _creatureData;
  
        #endregion

        protected float AttackDistance
        {
            get
            {
                //최소범위
                float radius = (_targetObject.Radius + Radius);//+ 1.0f);
                return radius + AttackRange.Value;

                // return AttackRange.Value + _targetObject.Radius;
            }
        }

        protected float DistToTargetSqr
        {
            get
            {
                Vector3 dir = (_targetObject.transform.position - transform.position);
                float distToTarget = Math.Max(0, dir.magnitude - _targetObject.ExtraSize * 1f - ExtraSize * 1f); // TEMP
                return distToTarget * distToTarget;
            }
        }

        protected override int SortingOrder => Define.SortingLayers.CREATURE;
        protected SkillBook _skillBook;
        protected EffectComponent _effectComponent;
        protected CancellationTokenSource _aiCts;
        protected BaseObject _targetObject;
        protected Queue<Vector3Int> _pathQueue = new Queue<Vector3Int>();
        [SerializeField] protected Define.CreatureState _creatureState = Define.CreatureState.None;

        private Vector3 _endMovePosition;
        private Vector3 _targetPosition;
        private CancellationTokenSource _moveCts; 
            
        private Coroutine _coWait = null;
        protected bool _isLerpCompleted = false;
        protected readonly float _chaseDistance = 8f;
        protected readonly float _searchDistance = 8f;
        protected readonly int _distanceToTargetThreshold = 5;
        
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
            
            _collider2D.offset = new Vector2(_creatureData.ColliderOffsetX, _creatureData.ColliderOffsetY);
            _collider2D.radius = _creatureData.ColliderRadius;

            SetCreatureStat();
            SetSpinAnimation(_creatureData.SkeletonDataID);
            _effectComponent = Util.GetOrAddComponent<EffectComponent>(gameObject);
            _effectComponent.SetInfo(this);
            
            _skillBook = Util.GetOrAddComponent<SkillBook>(gameObject);
            _skillBook.SetInfo(this);
            _skillBook.AddSkill(_creatureData.DefaultSkillId, Define.SkillType.DefaultSkill);
            _skillBook.AddSkill(_creatureData.EnvSkillId, Define.SkillType.EnvSkill);
            _skillBook.AddSkill(_creatureData.SkillAId, Define.SkillType.SkillA);
            _skillBook.AddSkill(_creatureData.SkillBId, Define.SkillType.SkillB);
            ChangeState(Define.CreatureState.Idle);
        }

        protected virtual void SetCreatureStat()
        {
            _currentHp = _creatureData.MaxHp;
            _maxHp = new CreatureStat(_creatureData.Atk);
            _atk = new CreatureStat(_creatureData.Atk);
            _attackRange = new CreatureStat(_creatureData.AtkRange);
            _moveSpeed = new CreatureStat(_creatureData.MoveSpeed);
            _atkBonus = new CreatureStat(_creatureData.AtkBonus);
            _reduceDamageRate = new CreatureStat(0);
            _thornsDamageRate = new CreatureStat(0);
            _attackSpeedRate = new CreatureStat(1);
            _criDamage = new CreatureStat(CreatureData.CriDamage);
            _criRate = new CreatureStat(CreatureData.CriRate);
            _lifeStealRate = new CreatureStat(0);
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
                    // if (ObjectType == Define.EObjectType.Hero)
                    // {
                    //     PlayAnimation(0, Define.AnimationName.Attack, false);
                    // }
                    // else
                    // {
                    //     PlayAnimation(0, Define.AnimationName.Attack_a, false);
                    // }
                    break;
                case Define.CreatureState.Dead:
                    PlayAnimation(0, Define.AnimationName.Dead, false);
                    break;
                case Define.CreatureState.Stun:

                    if (_animation.AnimationState.GetCurrent(0).Animation.Name != Define.AnimationName.Idle)
                    {
                        PlayAnimation(0, Define.AnimationName.Idle, false);    
                    }
                    
                    break;
            }
        }

        public override void SetCellPosition(Vector3Int cellPos, Vector3 cellWorldPos, bool forceMove = false)
        {
            base.SetCellPosition(cellPos, cellWorldPos, forceMove);
            if (forceMove)
            {
                if (_pathQueue.Count > 0)
                {
                    _pathQueue.Clear();
                }
            }
        }

        public override void Dead()
        {
            base.Dead();
            EffectComponent.RemoveAllSkillEffect();
            ChangeState(Define.CreatureState.Dead);
            Util.SafeCancelToken(ref _moveCts);
            _skillBook.StopAllSKill();
        }
        
        public override void TakeDamage(Creature attacker, SkillData skillData)
        {
            base.TakeDamage(attacker, skillData);
            
            float damage = attacker.Atk.Value * skillData.DamageMultiplier + attacker.AtkBonus.Value;
            bool isCritical = Random.value >= CriRate.Value;
            float finalDamage = isCritical ? damage * CriDamage.Value : damage;
            
            _currentHp -= (int) Mathf.Clamp(finalDamage, 0, finalDamage);
            if (_currentHp <= 0)
            {
                Dead();
            }
            
            ShowDamageFont(finalDamage, isCritical);
            ApplyEffect(skillData.EffectIds);
            
            if (skillData != null && skillData.AoEId != 0)
            {
                _skillBook.ExecuteAoESkill(transform.position, skillData);
            }
        }

        public void ApplyEffect(List<int> effectIdList)
        {
            if (effectIdList == null)
            {
                return;
            }
            
            foreach (int id in effectIdList)
            {
                EffectData effectData = Managers.Data.EffectDataDict[id];
                _effectComponent.ExecuteEffect(effectData);
            }
        }

        private void ShowDamageFont(float finalDamage, bool isCritical)
        {
            GameObject prefab = Managers.Resource.Instantiate("DamageFont");
            if (!prefab.TryGetComponent(out DamageFont damageFont))
            {
                return;
            }
            
            damageFont.SetInfo(transform.position + Vector3.up, finalDamage, null, isCritical);
        }

        protected BaseObject FindNearestCreatureInRange(float distance, IEnumerable<BaseObject> objectList)
        {
            float chaseDistance = _chaseDistance * _chaseDistance;
            float distA = distance * distance;
            BaseObject nearestObj = null;
            float nearestDistance = float.MaxValue;
            foreach (BaseObject obj in objectList)
            {
                float distB = (transform.position - obj.transform.position).sqrMagnitude;
                if (distB < distA && distB < nearestDistance && distB < chaseDistance)
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

            // if (trackEntry.Animation.Name == Define.AnimationName.Attack &&
            //     _creatureState == Define.CreatureState.Attack)
            // {
            //     PlayAnimation(0, Define.AnimationName.Attack, false);
            // }
            // else if (trackEntry.Animation.Name == Define.AnimationName.Attack_a &&
            //          _creatureState == Define.CreatureState.Attack)
            // {
            //     PlayAnimation(0, Define.AnimationName.Attack_a, false);
            // }
            // else if (trackEntry.Animation.Name == Define.AnimationName.Attack_b &&
            //          _creatureState == Define.CreatureState.Attack)
            // {
            //     PlayAnimation(0, Define.AnimationName.Attack_b, false);
            // }
            // else 
            //
            if (trackEntry.Animation.Name == Define.AnimationName.Dead &&
                     _creatureState == Define.CreatureState.Dead)
            {
               // StartCoroutine(Delay(1, () => Managers.Object.Despawn(this)));
            }
        }

        protected IEnumerator Delay(float delay, UnityAction callback = null)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
            _coWait = null;
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
                    case Define.CreatureState.Stun:
                        StunState();
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

        protected virtual void StunState()
        {
            _skillBook.StopAllSKill();
        }
        
        protected virtual void ChaseAndAttack() {}
        protected virtual void IdleState() {}
        protected virtual void MoveState(){}

        protected virtual void AttackState()
        {
            if (_coWait != null)
            {
                return;
            }
            
            _skillBook.UseSKill();
            TrackEntry trackEntry = _animation.AnimationState.GetCurrent(0);
            float delay = trackEntry.Animation.Duration;
            _coWait = StartCoroutine(Delay(delay));
         
            Vector3 direction = (_targetObject.transform.position - transform.position).normalized;
            SetFlip(Mathf.Sign(direction.x) == 1);
     
            //기존에 있던 모든 이동 경로는 지운다.
            if (_pathQueue.Count > 0)
            {
                _pathQueue.Clear();
            }
        }

        protected virtual void DeadState()
        {
            ChangeState(Define.CreatureState.Dead);
        }

        private List<Vector3Int> _reserveCellPos = new List<Vector3Int>();
        protected void SetMoveToCellPosition()
        {
            if (_pathQueue.Count == 0)
            {
                return;
            }
            
            foreach (Vector3Int reserveCellPo in _reserveCellPos)
            {
                Map.RemoveCellPosition(reserveCellPo, this);
            }

            _reserveCellPos.Clear();
            Vector3Int position = _pathQueue.Dequeue();
            
            for (int i = - ExtraSize; i <= ExtraSize; i++)
            {
                for (int j = -ExtraSize; j <= ExtraSize; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    
                    Vector3Int pos = new Vector3Int(position.x + i, position.y + j, 0);
                    _reserveCellPos.Add(pos);
                }
            }
            
            bool isPossibleMove = Map.MoveToCell(position, _cellPosition, this);
            // if (isPossibleMove)
            // {
            //     _cellPosition = position;
            // }
            
            foreach (Vector3Int reserveCellPo in _reserveCellPos)
            {
                if (isPossibleMove)
                {
                    Map.SetCellPosition(reserveCellPo, this);
                }
                else
                {
                    Map.RemoveCellPosition(reserveCellPo, this);
                }
            }
        }

        protected Define.PathFineResultType FindNextPath(BaseObject targetObj, int depth)
        {
            if (!_isLerpCompleted)
            {
                return Define.PathFineResultType.Lerp;
            }
            
            Vector3 targetPos = targetObj.transform.position;
            if ((targetPos - transform.position).sqrMagnitude < _distanceToTargetThreshold)
            {
                return Define.PathFineResultType.None;
            }

            Vector3Int startPosition = CellPosition;
            Vector3Int destPosition = Map.WorldToCell(targetPos);
            List<Vector3Int> list = Map.PathFinding(startPosition, destPosition, this, depth);
            if (list.Count < 2)
            {
                return Define.PathFineResultType.Fail;
            }
            
            Vector3Int nextPos = list[1];
            if(CanMoveToCell(nextPos, _cellPosition))
                return Define.PathFineResultType.Success;
            
            return Define.PathFineResultType.Fail;
        }

        protected Define.PathFineResultType FindPath(BaseObject targetObj, bool forceFindPath = false)
        {
            return FindPath(targetObj.transform.position, forceFindPath);
        }

        protected Define.PathFineResultType FindPath(Vector3 destinationPosition, bool forceFindPath = false, int depth = 5)
        {
            if (!_isLerpCompleted)
            {
                return Define.PathFineResultType.Lerp;
            }

            if (!forceFindPath && (destinationPosition - transform.position).sqrMagnitude < _distanceToTargetThreshold)
            {
                return Define.PathFineResultType.None;
            }
            
            Vector3Int startPosition = Map.WorldToCell(transform.position);
            Vector3Int destPosition = Map.WorldToCell(destinationPosition);
            List<Vector3Int> list = Map.PathFinding(startPosition, destPosition, this, depth);
            if (list.Count <= 2)
            {
                return Define.PathFineResultType.Fail;
            }
            
            _pathQueue = new Queue<Vector3Int>(list);
            _pathQueue.Dequeue();

            Vector3Int nextPosition = _pathQueue.Peek();
            if (!Map.MoveToCell(nextPosition, CellPosition, this))
            {
                return Define.PathFineResultType.FailToMove;
            }
            
            return Define.PathFineResultType.Success;
        }

        protected void StartMoveToCellPosition()
        {
            Util.SafeAllocateToken(ref _moveCts);
            MoveToCellPosition().Forget();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAIProcess();
            StopMoveToCellPosition();
        }

        #region Effect
        public void DotDamage(float amount)
        {
            float damage = amount;
            _currentHp -= (int) Mathf.Clamp(damage, 0, damage);
            if (_currentHp <= 0)
            {
                Dead();
            }
            
            ShowDamageFont(damage, false);
        }
        #endregion

        protected bool CanMoveToCell(Vector3Int nextPosition, Vector3Int prevPosition)
        {
            if (!_isLerpCompleted)
            {
                return false;
            }
            
            Vector3 myPos = transform.position;
            Vector3 cellPos = Managers.Map.CellToWorld(_cellPosition);
            return Map.MoveToCell(nextPosition, prevPosition, this) && (myPos - cellPos).sqrMagnitude <= 1f;
        }

        private async UniTaskVoid MoveToCellPosition()
        {
            while (_moveCts != null && _moveCts.IsCancellationRequested == false)
            {
                Vector3 myPos = transform.position;
                Vector3 cellPos = Managers.Map.CellToWorld(_cellPosition);
                while ((myPos - cellPos).sqrMagnitude >= 1f)
                {
                    _isLerpCompleted = false;
                    myPos = transform.position;
                    cellPos = Managers.Map.CellToWorld(_cellPosition);

                    float speed = MoveSpeed.Value;
                    if (this is Hero)
                    {
                        float multiplier = Mathf.Max(1, (cellPos - myPos).sqrMagnitude / 3);
                        speed *= multiplier;
                    }
                    
                    Vector3 dir = cellPos - myPos;
                    //일관성 있게 이동하기 위해서
                    float moveDist = Mathf.Min(dir.magnitude, speed * Time.deltaTime);
                    transform.position += dir.normalized * moveDist;
                    SetFlip(Mathf.Sign(dir.x) == 1);
                    
                    try
                    {
                        await UniTask.Yield(cancellationToken: _moveCts.Token);
                    }
                    catch (Exception e) when (e is not OperationCanceledException)
                    {
                        Util.SafeCancelToken(ref _moveCts);
                        return;
                    }
                }

                _isLerpCompleted = true;
                // SetMoveToCellPosition();
               
                try
                {
                    await UniTask.Yield(cancellationToken: _moveCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Util.SafeCancelToken(ref _moveCts);
                    return;
                }
            }
        }

        public Vector3Int _nextPos;
        public bool useGizmos = false;
        
        private void OnDrawGizmos()
        {
            if (_pathQueue == null)
            {
                return;
            }

            // if (!useGizmos)
            // {
            //     return;
            // }
            //
            // if (ObjectType == Define.EObjectType.Monster)
            // {
            //     Gizmos.color = Color.yellow;
            //
            //     Gizmos.DrawSphere(Managers.Map.CellToWorld(_cellPosition), 0.3f);
            //
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawSphere(Managers.Map.CellToWorld(_nextPos), 0.3f);
            // }
            //
            // else
            // {
            //     Gizmos.color = Color.gray;
            //
            //     Gizmos.DrawSphere(Managers.Map.CellToWorld(_cellPosition), 0.3f);
            //
            //     Gizmos.color = Color.green;
            //     Gizmos.DrawSphere(Managers.Map.CellToWorld(_nextPos), 0.3f);
            // }
            
            Gizmos.color = Color.gray;
            foreach (Vector3Int vector3Int in _pathQueue)
            {
                Gizmos.DrawSphere(Managers.Map.CellToWorld(vector3Int), 0.3f);
            }
        }
    }
}