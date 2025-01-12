using System;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Utils;

namespace Clicker.Effect
{
    public class BuffBase : EffectBase
    {
        public override void SetInfo(EffectData effectData, Action<EffectBase> onCompleteEffectAction)
        {
            base.SetInfo(effectData, onCompleteEffectAction);

            if (effectData.Amount < 0 || effectData.PercentAdd < 0)
            {
                _effectType = Define.EEffectType.Buff;
            }
            else
            {
                _effectType = Define.EEffectType.Debuff;
            }
        }
    }
}