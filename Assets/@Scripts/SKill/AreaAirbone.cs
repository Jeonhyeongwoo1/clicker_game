using System;
using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Skill
{
    public class AreaAirbone : AreaSkill
    {
        protected override float Angle => _angle;

        private float _angle = 360;

        public override void SetInfo(int id, Creature owner, Define.SkillType skillType,
            Action<BaseSKill> onCompleteSkillAction, Action<BaseSKill> onStopSkillAction)
        {
            base.SetInfo(id, owner, skillType, onCompleteSkillAction, onStopSkillAction);
            AddSpellIndicator();
        }

        protected override void OnAttackEvent()
        {
            base.OnAttackEvent();
            _spellIndicator.Cancel();
            List<BaseObject> list = FindTargetAndTakeDamage(transform.position,
                _skillData.SkillRange * _skillData.ScaleMultiplier, Angle,
                _direction, _skillData.SkillRange, LayerMask.GetMask("Hero", "Monster"));

            foreach (BaseObject obj in list)
            {
                if (obj.ObjectType != _onwer.ObjectType)
                {
                    obj.TakeDamage(_onwer, _skillData);
                }
            }
        }
    }
}