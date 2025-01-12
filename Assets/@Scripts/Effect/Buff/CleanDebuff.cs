using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Effect;
using Clicker.Utils;

namespace Clicker.Effect
{
    public class CleanDebuff : BuffBase
    {
        public override void ApplyEffect(Creature owner, EffectData effectData)
        {
            owner.EffectComponent.RemoveAllDebuffEffect();
            CompleteEffect(Define.EffectClearType.TimeOut);
        }
    }
}