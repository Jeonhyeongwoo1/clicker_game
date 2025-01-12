using System;
using System.Collections.Generic;
using System.Threading;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine;

namespace Clicker.Effect
{
    public class EffectBase : BaseObject
    {
        protected EffectData _effectData;
        protected Action _onApplyEffectAction;
        protected Creature _owner;
        
        private float _remainTime;
        private CancellationTokenSource _tickCts;
        private Action<EffectBase> _onCompleteEffectAction;
        
        public void SetInfo(EffectData effectData, Action<EffectBase> onCompleteEffectAction)
        {
            _effectData = effectData;
            _onCompleteEffectAction = onCompleteEffectAction;

            if (effectData.SkeletonDataID != null)
            {
                SetSpinAnimation(effectData.SkeletonDataID);
            }
            
            // _animation = GetComponent<SkeletonAnimation>();
            // if (_animation)
            // {
            //     _animation.skeletonDataAsset = null;
            // }   
        }
        
        public virtual void ApplyEffect(Creature owner, EffectData effectData)
        {
            _owner = owner;
            ExecuteTick().Forget();
        }

        private void ApplyBuffAndDebuff()
        {
            Define.BuffAndDebuffType type;
            try
            {
                type = Util.ParseEnum<Define.BuffAndDebuffType>(_effectData.ClassName);
            }
            catch (Exception e)
            {
                return;
            }

            CreatureStat creatureStat = null;
            switch (type)
            {
                case Define.BuffAndDebuffType.MoveSpeedBuff:
                    creatureStat = _owner.MoveSpeed;
                    break;
                case Define.BuffAndDebuffType.AttackSpeedBuff:
                    // creatureStat = _owner.AttackRange
                    break;
                case Define.BuffAndDebuffType.LifeStealBuff:
                    break;
            }

            if (creatureStat == null)
            {
                return;
            }
            
            
            if (_effectData.Amount != 0)
            {
                float amount = _effectData.Amount;
                StatModifer modifer = new StatModifer(Define.EStatModType.Add, amount, this, 0);
            }

            if (_effectData.PercentAdd != 0)
            {
                float percent = _effectData.PercentAdd;
                StatModifer modifer = new StatModifer(Define.EStatModType.PercentAdd, percent, this, 0);
            }

            if (_effectData.PercentMult != 0)
            {
                float percent = _effectData.PercentMult;
                StatModifer modifer = new StatModifer(Define.EStatModType.PercentMult, percent, this, 0);
            }
            
            
        }

        protected virtual void CompleteEffect(Define.EffectClearType effectClearType)
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