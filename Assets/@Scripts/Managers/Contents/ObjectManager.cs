using System.Collections.Generic;
using Clicker.Controllers;
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
        public HeroCamp HeroCamp => _heroCamp;
        
        private readonly HashSet<Creature> _heroSet = new();
        private readonly HashSet<Creature> _monsterSet = new();
        private readonly HashSet<Env> _envSet = new();
        private readonly HashSet<Projectile> _projectileSet = new();
        private HeroCamp _heroCamp;
        
        public T CreateObject<T>(Define.ObjectType objectType, int id) where T : BaseObject
        {
            string key = typeof(T).Name;
            GameObject prefab = Managers.Resource.Instantiate(key);
            if (!prefab.TryGetComponent(out BaseObject baseObject))
            {
                LogUtils.LogError("Failed get creature object : " + key);
                return null;
            }

            switch (objectType)
            {
                case Define.ObjectType.Hero:
                    Creature creature = baseObject as Creature;
                    _heroSet.Add(creature);
                    break;
                case Define.ObjectType.Monster:
                    creature = baseObject as Creature;
                    _monsterSet.Add(creature);
                    break;
                case Define.ObjectType.Env:
                    Env env = baseObject as Env;
                    _envSet.Add(env);
                    break;
                case Define.ObjectType.HeroCamp:
                    HeroCamp heroCamp = baseObject as HeroCamp;
                    _heroCamp = heroCamp;
                    break;
                case Define.ObjectType.Projectile:
                    Projectile projectile = baseObject as Projectile;
                    _projectileSet.Add(projectile);
                    break;
            }
            
            baseObject.Init(objectType);
            baseObject.SetInfo(id);
            return baseObject as T;
        }

        public void Despawn(BaseObject obj)
        {
            switch (obj.ObjectType)
            {
                case Define.ObjectType.Hero:
                    _heroSet.Remove(obj as Creature);
                    break;
                case Define.ObjectType.Monster:
                    _monsterSet.Remove(obj as Creature);
                    break;
                case Define.ObjectType.Env:
                    _envSet.Remove(obj as Env);
                    break;
                case Define.ObjectType.Projectile:
                    _projectileSet.Remove(obj as Projectile);
                    break;
            }
            
            Managers.Resource.Destroy(obj.gameObject);
        }

        public HashSet<Creature> GetCreatureList(Define.ObjectType objectType)
        {
            switch (objectType)
            {
                case Define.ObjectType.Hero:
                    return _monsterSet;
                case Define.ObjectType.Monster:
                    return _heroSet;
            }

            LogUtils.LogError("Failed get creature list" + objectType);
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