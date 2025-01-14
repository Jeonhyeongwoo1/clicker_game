using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.Skill
{
    public class BaseAoE : BaseObject
    {
        private AoEData _aoeData;
        private CancellationTokenSource _cts;

        protected override int SortingOrder => Define.SortingLayers.AOE;
        protected float radius = 3;
        
        private Collider2D[] _collider2Ds = new Collider2D[20];
        private SkillData _skillData;
        private Creature _owner;

        public virtual void SetInfo(int id, SkillData skillData, Creature owner)
        {
            _owner = owner;
            _skillData = skillData;
            
            _aoeData = Managers.Data.AoEDataDict[id];
            SetSpinAnimation(_aoeData.SkeletonDataID);
            _collider2D.radius = radius = Util.GetEffectRadius((Define.EEffectSize)skillData.EffectSize);
            _collider2D.isTrigger = true;
            Util.SafeAllocateToken(ref _cts);
            float duration = _aoeData.Duration;
            LifeCycle(duration).Forget();
            FindCreatureInCircleRange().Forget(); 

            PlayAnimation(0, Define.AnimationName.Idle, false);
        }
        
        protected virtual async UniTaskVoid FindCreatureInCircleRange()
        {
            HashSet<Collider2D> currentSet = new();
            HashSet<Collider2D> newSet = new();

            while (!_cts.IsCancellationRequested)
            {
                int cnt = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _collider2Ds,
                    LayerMask.GetMask("Monster", "Hero"));

                newSet.Clear();
                for (int i = 0; i < cnt; i++)
                {
                    Collider2D col = _collider2Ds[i];
                    if (col.transform != transform)
                    {
                        newSet.Add(col);
                    }
                }

                // 기존에 있던 요소 중 제거
                foreach (var col in currentSet.Except(newSet))
                {
                    Creature creature = col.GetComponent<Creature>();
                    List<int> effectIdList = creature.ObjectType == _owner.ObjectType
                        ? _aoeData.EnemyEffects
                        : _aoeData.AllyEffects;

                    ExecuteEffect(creature, effectIdList, false);
                }

                // 새롭게 추가된 요소 처리
                foreach (var col in newSet.Except(currentSet))
                {
                    Creature creature = col.GetComponent<Creature>();
                    List<int> effectIdList = creature.ObjectType == _owner.ObjectType
                        ? _aoeData.EnemyEffects
                        : _aoeData.AllyEffects;

                    ExecuteEffect(creature, effectIdList, true);
                }

                currentSet = new HashSet<Collider2D>(newSet);

                try
                {
                    await UniTask.WaitForSeconds(1f, cancellationToken: _cts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"{typeof(BaseObject)} / {nameof(FindCreatureInCircleRange)} / error : {e.Message}");
                    break;
                }
            }
        }

        private void ExecuteEffect(Creature creature, List<int> effectIdList, bool isExecuteEffect)
        {
            if (effectIdList == null || effectIdList.Count == 0)
            {
                return;
            }
            
            if (!creature.IsValid())
            {
                return;
            }
            
            foreach (int aoeDataEnemyEffect in effectIdList)
            {
                EffectData effectData = Managers.Data.EffectDataDict[aoeDataEnemyEffect];

                if (isExecuteEffect)
                {
                    //Debug.Log($"add {creature.name} / {aoeDataEnemyEffect}");
                    creature.EffectComponent.ExecuteEffect(effectData);
                }
                else
                {
                    //Debug.Log($"remove {creature.name} / {aoeDataEnemyEffect}");
                    creature.EffectComponent.RemoveEffect(aoeDataEnemyEffect);
                }
            }
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            Util.SafeCancelToken(ref _cts);
        }

        private async UniTaskVoid LifeCycle(float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                try
                {
                    await UniTask.Yield(cancellationToken: _cts.Token);
                }
                catch (Exception e) when(e is not OperationCanceledException)
                {
                    LogUtils.LogError($"{typeof(BaseObject)} / {nameof(LifeCycle)} / error : {e.Message}");
                    break;
                }
            }
            
            Destory();
        }

        protected virtual void Destory()
        {
            Managers.Object.Despawn(this);
        }
    }
}