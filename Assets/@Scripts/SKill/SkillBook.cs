using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Manger;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Skill
{
    public class SkillBook : MonoBehaviour
    {
        private List<BaseSKill> _skillList = new List<BaseSKill>();
        
        public void AddSkill(List<int> skillIdList)
        {
            DataManager dataManager = Managers.Data;
            foreach (var id in skillIdList)
            {
                string namespaceString = "Clicker.Skill";
                string className = dataManager.SkillDataDict[id].ClassName;
                string name = $"{namespaceString}.{className}";
                var skill = gameObject.AddComponent(System.Type.GetType(name)) as BaseSKill;
                if (skill == null)
                {
                    LogUtils.LogError("Failed add skill " + id);
                    continue;
                }
                
                skill.Init(Define.ObjectType.Skill);
                skill.SetInfo(id);
                _skillList.Add(skill);
            }
        }

        public void UseSKill(Creature owner)
        {
            foreach (BaseSKill baseSKill in _skillList)
            {
                baseSKill.StartSkillProcess(owner);
            }
        }

        public void StopSkill()
        {
            foreach (BaseSKill baseSKill in _skillList)
            {
                baseSKill.StopSkill();
            }
        }
    }
}