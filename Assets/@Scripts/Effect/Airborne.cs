using System;
using System.Collections.Generic;
using System.Threading;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.Effect
{
    public class Airborne : EffectBase
    {
        private CancellationTokenSource _airbonEffectCts;
        private Define.CreatureState _lastCreatureState;
        
        public override void ApplyEffect(Creature owner, EffectData effectData)
        {
            base.ApplyEffect(owner, effectData);

            _lastCreatureState = _owner.CreatureState;
            _owner.ChangeState(Define.CreatureState.Stun);
            Util.SafeAllocateToken(ref _airbonEffectCts);
            AirboneEffect(effectData).Forget();
        }

        private async UniTask AirboneEffect(EffectData effectData)
        {
            float duration = effectData.TickTime;
            float elapsedTime = 0;
            float height = 10f;

            Vector3 startPos = _owner.transform.position;
            
            while (elapsedTime < duration)
            {
                elapsedTime = Mathf.Min(elapsedTime + Time.deltaTime, duration);
                float normalizedTime = elapsedTime / duration;
                float y = height * (1 - normalizedTime) * normalizedTime;

                Vector3 endPos = startPos + new Vector3(0, height, 0);
                Vector3 midPoint = Vector3.Lerp(startPos, endPos, normalizedTime);
                midPoint.y = startPos.y + y;
                _owner.transform.position = midPoint;
                
                try
                {
                     await UniTask.Yield(cancellationToken: _airbonEffectCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"{nameof(AirboneEffect)}: Unexpected error - {e.Message}");
                    return;
                }
            }

            CompleteEffect(Define.EffectClearType.EndOfAirbone);
        }

        public override void CompleteEffect(Define.EffectClearType effectClearType)
        {
            base.CompleteEffect(effectClearType);

            if (_owner.CreatureState != _lastCreatureState)
            {
                _owner.ChangeState(_lastCreatureState);
            }
        }
    }
}