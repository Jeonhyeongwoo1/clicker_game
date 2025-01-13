using System;
using System.Collections.Generic;
using System.Threading;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.Effect
{
    public class EffectBase : BaseObject
    {
        public int Id => _effectData.DataId;
        public Define.EEffectType EEffectType => _effectType;
        protected override int SortingOrder => Define.SortingLayers.SKILL_EFFECT;

        protected EffectData _effectData;
        protected Action _onApplyEffectAction;
        protected Creature _owner;
        protected Define.EEffectType _effectType;
        
        private float _remainTime;
        private CancellationTokenSource _tickCts;
        private List<CreatureStat> _creatureStatList = new();
        private Action<EffectBase> _onCompleteEffectAction;

        public override bool Init(Define.EObjectType eObjectType)
        {
            return base.Init(eObjectType);
        }

        public virtual void SetInfo(EffectData effectData, Action<EffectBase> onCompleteEffectAction)
        {
            _effectData = effectData;
            _onCompleteEffectAction = onCompleteEffectAction;

            if (effectData.SkeletonDataID != null)
            {
                SetSpinAnimation(effectData.SkeletonDataID);
            }
        }
        
        public virtual void ApplyEffect(Creature owner, EffectData effectData)
        {
            _owner = owner;
            ExecuteTick().Forget();
        }

        protected void ApplyBuff(CreatureStat creatureStat, object source, int order = 0)
        {
            if (creatureStat == null)
            {
                LogUtils.LogError("stat is null");
                return;
            }
            
            if (_effectData.Amount != 0)
            {
                float amount = _effectData.Amount;
                StatModifer modifer = new StatModifer(Define.EStatModType.Add, amount, source, order);
                creatureStat.AddStat(modifer);
            }

            if (_effectData.PercentAdd != 0)
            {
                float percent = _effectData.PercentAdd;
                StatModifer modifer = new StatModifer(Define.EStatModType.PercentAdd, percent, source, order);
                creatureStat.AddStat(modifer);
            }

            if (_effectData.PercentMult != 0)
            {
                float percent = _effectData.PercentMult;
                StatModifer modifer = new StatModifer(Define.EStatModType.PercentMult, percent, source, order);
                creatureStat.AddStat(modifer);
            }
        }

        protected void RemoveBuff(CreatureStat creatureStat, object source)
        {
            creatureStat.RemoveStatBySource(source);
        }
        
        public virtual void CompleteEffect(Define.EffectClearType effectClearType)
        {
            _remainTime = 0;
            Util.SafeCancelToken(ref _tickCts);

            switch (effectClearType)
            {
                case Define.EffectClearType.TimeOut:
                case Define.EffectClearType.EndOfAirbone:
                    break;
            }
            
            _onCompleteEffectAction?.Invoke(this);
        }
        
        protected void DotDamage()
        {
            _owner.DotDamage(_effectData.Amount);
        }

        protected async UniTaskVoid ExecuteTick()
        {
            Util.SafeAllocateToken(ref _tickCts);
            _remainTime = _effectData.TickCount * _effectData.TickTime;
            float sumTime = 0;
            while(_remainTime > 0)
            {
                _remainTime -= Time.deltaTime;
                sumTime += Time.deltaTime;

                if (sumTime >= _effectData.TickTime)
                {
                    DotDamage();
                    _onApplyEffectAction?.Invoke();
                    sumTime -= _effectData.TickTime;
                }
                
                try
                {
                    await UniTask.Yield(_tickCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"{nameof(e)} /  {e.Message}");
                    return;
                }
            }

            CompleteEffect(Define.EffectClearType.TimeOut);
        }
    }
}