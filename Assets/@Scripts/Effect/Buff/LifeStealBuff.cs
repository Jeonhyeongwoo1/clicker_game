using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Effect;
using Clicker.Utils;

namespace Clicker.Effect
{
    public class LifeStealBuff : BuffBase
    {
        public override void ApplyEffect(Creature owner, EffectData effectData)
        {
            base.ApplyEffect(owner, effectData);
            ApplyBuff(_owner.LifeStealRate, this);
        }

        public override void CompleteEffect(Define.EffectClearType effectClearType)
        {
            base.CompleteEffect(effectClearType);
            RemoveBuff(_owner.LifeStealRate, this);
        }
    }
}