using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Manger;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Effect
{
    public class EffectComponent : MonoBehaviour
    {

        [SerializeField] private List<EffectBase> _effectList = new();
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
            effect.transform.SetParent(transform);
            effect.transform.localPosition = Vector3.zero;
            Managers.Object.EffectSet.Add(effect);
            
            _effectList.Add(effect);
            effect.SetInfo(Managers.Data.EffectDataDict[id], CompleteEffect);
            effect.ApplyEffect(_owner, effectData);
        }

        public void CompleteEffect(EffectBase effect)
        {
            if (_effectList.Contains(effect))
            {
                _effectList.Remove(effect);
            }
            
            Managers.Object.Despawn(effect);
        }
        
    }
}