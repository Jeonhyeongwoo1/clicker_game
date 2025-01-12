using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Utils;

namespace Clicker.Effect
{
    public class MoveSpeedBuff : BuffBase
    {
        public override void ApplyEffect(Creature owner, EffectData effectData)
        {
            base.ApplyEffect(owner, effectData);
            ApplyBuff(_owner.MoveSpeed, this);
        }

        public override void CompleteEffect(Define.EffectClearType effectClearType)
        {
            base.CompleteEffect(effectClearType);
            RemoveBuff(_owner.MoveSpeed, this);
        }
    }
}