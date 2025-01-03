using System;
using System.Collections.Generic;
using System.Linq;
using Clicker.ConfigData;
using Clicker.ContentData.Data;
using Clicker.Controllers;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Manager
{
    public class ObjectManager
    {
        private readonly HashSet<Creature> _heroSet = new();
        private readonly HashSet<Creature> _monsterSet = new();
        
        public T CreateCreature<T>(Define.ObjectType objectType, int id) where T : Creature
        {
            string key = typeof(T).Name;
            GameObject prefab = Managers.Resource.Instantiate(key);
            if (!prefab.TryGetComponent(out Creature creature))
            {
                LogUtils.LogError("Failed get creature object : " + key);
                return null;
            }

            CreatureData creatureData = Managers.Data.CreatureDataDict[id];
            switch (objectType)
            {
                case Define.ObjectType.Hero:
                    _heroSet.Add(creature);
                    break;
                case Define.ObjectType.Monster:
                    _monsterSet.Add(creature);
                    break;
            }
            
            creature.Init(objectType);
            creature.SetInfo(creatureData);
            return creature as T;
        }

        public HashSet<Creature> GetHeroList()
        {
            return _heroSet;
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