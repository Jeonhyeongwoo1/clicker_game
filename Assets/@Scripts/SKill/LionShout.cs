using System.Collections.Generic;
using System.Linq;
using Clicker.Controllers;
using Clicker.Entity;
using UnityEngine;

namespace Clicker.Skill
{
    public class LionShout : AreaSkill
    {
        protected override float Angle => 360f;

        protected override void OnAttackEvent()
        {
            CoolTimeAsync().Forget();
            _spellIndicator.Cancel();
            List<BaseObject> list = FindTargetAndTakeDamage(transform.position,
                _skillData.SkillRange * _skillData.ScaleMultiplier, Angle,
                _direction, _skillData.SkillRange, LayerMask.GetMask("Hero"));

            foreach (var creature in list.OfType<Creature>())
            {
                creature.ApplyEffect(_skillData.EffectIds);
            }
        }
    }
}