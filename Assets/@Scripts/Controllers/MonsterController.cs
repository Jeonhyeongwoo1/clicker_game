using System;
using System.Collections.Generic;
using System.Threading;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Exception = System.Exception;

namespace Clicker.Controllers
{
    public class MonsterController : Creature
    {
        [SerializeField] private Define.MonsterState _monsterState;

        private float _searchDistance = 9f;
        private float _attackDistance = 4f;
        private CancellationTokenSource _aiCts;                
        private Creature _targetHero = null;
        private Vector3 _spawnPos = Vector3.zero;

        protected static class AnimationName
        {
            public static string Idle = "idle";
            public static string Move = "move";
            public static string Attack_a = "attack_a";
            public static string Attack_b = "attack_b";
            public static string Dead = "dead";
        }
        
        public override bool Init(Define.CreatureType creatureType)
        {
            ChangeState(Define.MonsterState.Idle);
            
            return base.Init(creatureType);
        }

        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
            _aiCts = new CancellationTokenSource();
            StartAiProcessAsync().Forget();

            _spawnPos = spawnPosition;
        }

        private void Update()
        {
        }
        
        private async UniTaskVoid StartAiProcessAsync()
        {
            while (!_aiCts.IsCancellationRequested)
            {
                List<Creature> heroList = Managers.Object.GetHeroList();
                switch (_monsterState)
                {
                    case Define.MonsterState.Idle:
                        await IdleStateProcess(heroList);
                        break;
                    case Define.MonsterState.Move:
                        await MoveStateProcess();
                        break;
                    case Define.MonsterState.Attack:
                        await AttackProcess();
                        break;
                    case Define.MonsterState.Dead:
                        break;
                }
                
                try
                {
                    await UniTask.WaitForSeconds(1, cancellationToken: _aiCts.Token);
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
                float distA = (transform.position - _targetHero.transform.position).sqrMagnitude;
                float distC = _attackDistance * _attackDistance;
                bool isAttackableHero = distC > distA;
                if (!isAttackableHero)
                {
                    ChangeState(Define.MonsterState.Move);
                    return;
                }
                
                await UniTask.WaitForSeconds(_attackCoolTime, cancellationToken: _aiCts.Token);
            }
        }

        private void ChangeState(Define.MonsterState state)
        {
            if (state == _monsterState)
            {
                return;
            }

            Debug.Log("Change state :" + state);
            _monsterState = state;
            switch (state)
            {
                case Define.MonsterState.Idle:
                    ChangeAnimation(AnimationName.Idle);
                    break;
                case Define.MonsterState.Move:
                    ChangeAnimation(AnimationName.Move);
                    break;
                case Define.MonsterState.Attack:
                    ChangeAnimation(AnimationName.Attack_a);
                    break;
                case Define.MonsterState.Dead:
                    ChangeAnimation(AnimationName.Dead);
                    break;
            }
        }

        private async UniTask MoveStateProcess()
        {
            while (!_aiCts.IsCancellationRequested)
            {
                if (_targetHero == null)
                {
                    ChangeState(Define.MonsterState.Idle);
                    return;
                }
                
                float distA = (transform.position - _targetHero.transform.position).sqrMagnitude;
                float distB = _searchDistance * _searchDistance;
                bool isDefectedHero = distA < distB;

                if (!isDefectedHero)
                {
                    try
                    {
                        await UniTask.WaitForSeconds(0.5f, cancellationToken: _aiCts.Token);
                    }
                    catch (Exception e) when (e is not OperationCanceledException)
                    {
                        LogUtils.LogError($"Error {nameof(StartAiProcessAsync)} / {e.Message}");
                    }
                    
                    ChangeState(Define.MonsterState.Idle);
                    _targetHero = null;
                    return;
                }

                float distC = _attackDistance * _attackDistance;
                bool isAttackableHero = distC > distA;
                if (isAttackableHero)
                {
                    ChangeState(Define.MonsterState.Attack);
                    return;
                }

                MoveTo(_targetHero.transform.position);
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
        }
        
        private async UniTask IdleStateProcess(List<Creature> heroList)
        {
            while (!_aiCts.IsCancellationRequested)
            {
                float dist = Vector2.Distance(_spawnPos, transform.position);
                //Debug.Log($"Distance : {dist}");
                if (dist < 0.1f)
                {  
                    foreach (Creature hero in heroList)
                    {
                        float distA = (transform.position - hero.transform.position).sqrMagnitude;
                        float distB = _searchDistance * _searchDistance;
                        bool isDefectedHero = distA < distB;

                        if (isDefectedHero)
                        {
                            _targetHero = hero;
                            ChangeState(Define.MonsterState.Move);
                            return;
                        }
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
                else
                {
                    MoveTo(_spawnPos);
                                    
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
}