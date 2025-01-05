using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.GameComponent
{
    public class AIComponent : MonoBehaviour
    {

        private float _searchDistance = 9f;
        private float _attackDistance = 4f;
        
        private CancellationTokenSource _aiCts;                
        private Creature _targetCreature = null;
        private Vector3 _originPosition;
        private Creature _owner;

        public void Initialize(Creature creature)
        {
            _owner = creature;
        }

        public void SetOriginPosition(Vector3 originPosition)
        {
            _originPosition = originPosition;
        }

        public void StartAiProcess()
        {
            _aiCts = new CancellationTokenSource();
            StartAiProcessAsync().Forget();
        }

        public void StopAIProcess()
        {
            if (_aiCts != null)
            {
                _aiCts.Cancel();
                _aiCts = null;
            }
        }
        
        private async UniTaskVoid StartAiProcessAsync()
        {
            while (!_aiCts.IsCancellationRequested)
            {
                HashSet<Creature> targets = Managers.Object.GetCreatureList(_owner.ObjectType);
                switch (_owner.CreatureState)
                {
                    case Define.CreatureState.Idle:
                        await IdleStateProcess(targets);
                        break;
                    case Define.CreatureState.Move:
                        await MoveStateProcess();
                        break;
                    case Define.CreatureState.Attack:
                        await AttackProcess();
                        break;
                    case Define.CreatureState.Dead:
                        break;
                }
                
                try
                {
                    await UniTask.WaitForSeconds(0.3f, cancellationToken: _aiCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"Error {nameof(StartAiProcessAsync)} / {e.Message}");
                }
            }
        }

        private async UniTask AttackProcess()
        {
            while (!_aiCts.IsCancellationRequested)
            {
                float distA = (transform.position - _targetCreature.transform.position).sqrMagnitude;
                float distC = _attackDistance * _attackDistance;
                bool isAttackableHero = distC > distA;
                if (!isAttackableHero)
                {
                    _owner.ChangeState(Define.CreatureState.Move);
                    return;
                }
                
                _owner.UseSKill();
                await UniTask.WaitForSeconds(_owner.GetTick(Define.CreatureState.Attack), cancellationToken: _aiCts.Token);
            }
        }
        
        private async UniTask MoveStateProcess()
        {
            while (!_aiCts.IsCancellationRequested)
            {
                if (_targetCreature == null)
                {
                    _owner.ChangeState(Define.CreatureState.Idle);
                    return;
                }
                
                float distA = (transform.position - _targetCreature.transform.position).sqrMagnitude;
                float distB = _searchDistance * _searchDistance;
                bool isDefectedHero = distA < distB;

                if (!isDefectedHero)
                {
                    float dist = Vector2.Distance(_originPosition, transform.position);
                    if (dist > 0.1f)
                    {
                        MoveTo(_originPosition);
                        await UniTask.Yield(cancellationToken:_aiCts.Token);
                        continue;
                    }
                    try
                    {
                        await UniTask.WaitForSeconds(0.5f, cancellationToken: _aiCts.Token);
                    }
                    catch (Exception e) when (e is not OperationCanceledException)
                    {
                        LogUtils.LogError($"Error {nameof(StartAiProcessAsync)} / {e.Message}");
                    }
                    
                    _owner.ChangeState(Define.CreatureState.Idle);
                    _targetCreature = null;
                    return;
                }

                float distC = _attackDistance * _attackDistance;
                bool isAttackableHero = distC > distA;
                if (isAttackableHero)
                {
                    _owner.ChangeState(Define.CreatureState.Attack);
                    return;
                }

                MoveTo(_targetCreature.transform.position);
                try
                {
                    await UniTask.Yield(cancellationToken: _aiCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"Error {nameof(StartAiProcessAsync)} / {e.Message}");
                }
            }
        }

        private void MoveTo(Vector3 targetPos)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            Vector3 myPos = transform.position;
            Vector3 moveLerp = Vector3.Lerp(myPos, myPos + direction, Time.deltaTime * 2.5f);
            transform.position = moveLerp;
            _owner.SetFlip(Mathf.Sign(direction.x) == 1);
        }
        
        private async UniTask IdleStateProcess(HashSet<Creature> creatureList)
        {
            while (!_aiCts.IsCancellationRequested)
            {
                foreach (Creature targetCreature in creatureList)
                {
                    float distA = (transform.position - targetCreature.transform.position).sqrMagnitude;
                    float distB = _searchDistance * _searchDistance;
                    bool isDefectedHero = distA < distB;

                    if (isDefectedHero)
                    {
                        _targetCreature = targetCreature;
                        _owner.ChangeState(Define.CreatureState.Move);
                        return;
                    }
                }

                float dist = Vector2.Distance(_originPosition, transform.position);
                if (dist > 0.1f)
                {  
                    _owner.ChangeState(Define.CreatureState.Move);
                    return;
                }

                try
                {
                    await UniTask.WaitForSeconds(0.3f, cancellationToken: _aiCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"Error {nameof(StartAiProcessAsync)} / {e.Message}");
                }
            }
        }
    }
} 