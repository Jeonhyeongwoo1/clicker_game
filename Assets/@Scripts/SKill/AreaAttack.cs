using System;
using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Skill
{
    public class AreaAttack : AreaSkill
    {
        protected override float Angle => _angle;

        private float _angle = 90;


        protected override void OnAttackEvent()
        {
            CoolTimeAsync().Forget();
            _spellIndicator.Cancel();
            List<BaseObject> list = FindTargetAndTakeDamage(transform.position,
                _skillData.SkillRange * _skillData.ScaleMultiplier, Angle,
                _direction, _skillData.SkillRange, LayerMask.GetMask("Monster"));

            foreach (BaseObject obj in list)
            {
                obj.TakeDamage(_onwer, _skillData);
            }
        }
    }
}