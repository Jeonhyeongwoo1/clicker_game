using System;
using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Manger;
using Clicker.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Clicker.Skill
{
    public class SkillBook : MonoBehaviour
    {
        public BaseSKill CurrentSkill
        {
            get
            {
                if (_activeSkillList.Count == 0)
                {
                    return _defaultSkill;
                }
                
                int select = Random.Range(0, _activeSkillList.Count);
                // Debug.Log($"{_activeSkillList[select].SkillType} /  {select} / {_activeSkillList.Count}");
                return _activeSkillList[select];
            }
        }
        
        private List<BaseSKill> _skillList = new List<BaseSKill>();
        [SerializeField] private List<BaseSKill> _activeSkillList = new List<BaseSKill>();
        private Creature _owner;
        
        private BaseSKill _defaultSkill;
        private BaseSKill _envSkill;
        private BaseSKill _skillA;
        private BaseSKill _skillB;
        
        public void SetInfo(Creature owner)
        {
            _owner = owner;
        }
        
        public void AddSkill(int id, Define.SkillType skillType)
        {
            DataManager dataManager = Managers.Data;
            string namespaceString = "Clicker.Skill";

            //TODO : Remove
            if (!dataManager.SkillDataDict.TryGetValue(id, out SkillData skillData))
            {
                return;
            }
            
            string className = dataManager.SkillDataDict[id].ClassName;
            string name = $"{namespaceString}.{className}";
            var skill = gameObject.AddComponent(System.Type.GetType(name)) as BaseSKill;
            if (skill == null)
            {
                LogUtils.LogError("Failed add skill " + id);
                return;
            }
            
            switch (skillType)
            {
                case Define.SkillType.DefaultSkill:
                    _defaultSkill = skill;
                    break;
                case Define.SkillType.EnvSkill:
                    _envSkill = skill;
                    break;
                case Define.SkillType.SkillA:
                    _skillA = skill;
                    break;
                case Define.SkillType.SkillB:
                    _skillB = skill;
                    break;
            }
            
            _activeSkillList.Add(skill);
            _skillList.Add(skill);
            skill.SetInfo(id, _owner, skillType, OnCompleteSkill, OnStopSKill);
        }

        public void UseSKill()
        {
            BaseSKill skill = CurrentSkill;
            if (_activeSkillList.Contains(skill))
            {
                _activeSkillList.Remove(skill);
            }

            if (skill.Id == 10019)
            {
                int a = 0;
            }
            Debug.LogError("USeSKill" + skill.Id);
            skill.StartSkillProcess();
        }

        private void OnCompleteSkill(BaseSKill skill)
        {
        }
        
        private void OnStopSKill(BaseSKill skill)
        {
            _activeSkillList.Add(skill);
        }
        
        public void StopAllSKill()
        {
            foreach (BaseSKill baseSKill in _skillList)
            {
                baseSKill.StopSkill();
            }
        }
    }
}