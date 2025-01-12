using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Manger;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clicker.Effect
{
    public class EffectComponent : MonoBehaviour
    {

        [FormerlySerializedAs("_effectList")] [SerializeField] private List<EffectBase> _activateEffectList = new();
        private Creature _owner;
        
        public virtual void SetInfo(Creature owner)
        {
            _owner = owner;
        }

        public void ExecuteEffect(EffectData effectData)
        {
            DataManager dataManager = Managers.Data;

            int id = effectData.DataId;
            string namespaceString = "Clicker.Effect";
            string className = dataManager.EffectDataDict[id].ClassName;
            string name = $"{namespaceString}.{className}";

            GameObject go = Managers.Object.SpawnGameObject(_owner.transform.position, "EffectBase");
            var effect = go.AddComponent(System.Type.GetType(name)) as EffectBase;
            effect.Init(Define.EObjectType.Effect);
            
            effect.transform.SetParent(transform);
            effect.transform.localPosition = Vector3.zero;
            effect.transform.name = className;
            
            Managers.Object.EffectSet.Add(effect);
            _activateEffectList.Add(effect);
            effect.SetInfo(Managers.Data.EffectDataDict[id], CompleteEffect);
            effect.ApplyEffect(_owner, effectData);
        }

        public void RemoveAllDebuffEffect()
        {
            for (int i = _activateEffectList.Count - 1; i >= 0; i--)
            {
                EffectBase effectBase = _activateEffectList[i];
                if (effectBase.EEffectType == Define.EEffectType.Debuff)
                {
                    effectBase.CompleteEffect(Define.EffectClearType.ClearSkill);
                }
            }
        }

        public void CompleteEffect(EffectBase effect)
        {
            if (_activateEffectList.Contains(effect))
            {
                _activateEffectList.Remove(effect);
            }
            
            Managers.Object.Despawn(effect);
        }
        
    }
}