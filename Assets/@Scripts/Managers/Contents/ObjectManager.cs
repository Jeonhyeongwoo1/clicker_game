using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Effect;
using Clicker.Entity;
using Clicker.Skill;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Manager
{
    public class ObjectManager
    {
        public HashSet<Creature> HeroSet => _heroSet;
        public HashSet<Creature> MonsterSet => _monsterSet;
        public HashSet<Env> EnvSet => _envSet;
        public HashSet<EffectBase> EffectSet => _effectSet;
        public HeroCamp HeroCamp => _heroCamp;
        
        private readonly HashSet<Creature> _heroSet = new();
        private readonly HashSet<Creature> _monsterSet = new();
        private readonly HashSet<Env> _envSet = new();
        private readonly HashSet<Projectile> _projectileSet = new();
        private readonly HashSet<EffectBase> _effectSet = new();
        private HeroCamp _heroCamp;
        
        public GameObject SpawnGameObject(Vector3 position, string prefabName)
        {
            GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
            go.transform.position = position;
            return go;
        }
        
        public T CreateObject<T>(Define.EObjectType eObjectType, int id) where T : BaseObject
        {
            string key = typeof(T).Name;
            GameObject prefab = Managers.Resource.Instantiate(key);
            if (!prefab.TryGetComponent(out BaseObject baseObject))
            {
                LogUtils.LogError("Failed get creature object : " + key);
                return null;
            }

            switch (eObjectType)
            {
                case Define.EObjectType.Hero:
                    Creature creature = baseObject as Creature;
                    _heroSet.Add(creature);
                    break;
                case Define.EObjectType.Monster:
                    creature = baseObject as Creature;
                    _monsterSet.Add(creature);
                    break;
                case Define.EObjectType.Env:
                    Env env = baseObject as Env;
                    _envSet.Add(env);
                    break;
                case Define.EObjectType.HeroCamp:
                    HeroCamp heroCamp = baseObject as HeroCamp;
                    _heroCamp = heroCamp;
                    break;
                case Define.EObjectType.Projectile:
                    Projectile projectile = baseObject as Projectile;
                    _projectileSet.Add(projectile);
                    break;
            }

            baseObject.name = prefab.name; 
            baseObject.Init(eObjectType);
            baseObject.SetInfo(id);
            return baseObject as T;
        }

        public void Despawn(BaseObject obj)
        {
            switch (obj.ObjectType)
            {
                case Define.EObjectType.Hero:
                    _heroSet.Remove(obj as Creature);
                    break;
                case Define.EObjectType.Monster:
                    _monsterSet.Remove(obj as Creature);
                    break;
                case Define.EObjectType.Env:
                    _envSet.Remove(obj as Env);
                    break;
                case Define.EObjectType.Projectile:
                    _projectileSet.Remove(obj as Projectile);
                    break;
                case Define.EObjectType.Effect:
                    _effectSet.Remove(obj as EffectBase);
                    break;
            }
            
            Managers.Resource.Destroy(obj.gameObject);
        }

        public HashSet<Creature> GetCreatureList(Define.EObjectType eObjectType)
        {
            switch (eObjectType)
            {
                case Define.EObjectType.Hero:
                    return _monsterSet;
                case Define.EObjectType.Monster:
                    return _heroSet;
            }

            LogUtils.LogError("Failed get creature list" + eObjectType);
            return null;
        }

        public Hero FindNearestHero(Vector3 position)
        {
            float distance = float.MaxValue;
            Creature selectedCreature = null;
            
            foreach (Creature hero in _heroSet)
            {
                float distA = (hero.transform.position - position).sqrMagnitude;
                if (distance > distA)
                {
                    distA = distance;
                    selectedCreature = hero;
                }
            }

            return selectedCreature as Hero;
        }
    }
}