using System.Collections.Generic;
using System.Linq;
using Clicker.Controllers;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Manager
{
    public class ObjectManager
    {
        private readonly Dictionary<int, Creature> _creatureDict = new Dictionary<int, Creature>();

        public Creature CreateCreature(string key, Define.CreatureType creatureType)
        {
            GameObject prefab = Managers.Resource.Instantiate(key);
            int id = prefab.transform.GetInstanceID();
            if (!prefab.TryGetComponent(out Creature creature))
            {
                LogUtils.LogError("Failed get creature object : " + key);
                return null;
            }
            
            _creatureDict.Add(id, creature);
            creature.Init(creatureType);
            return creature;
        }

        public List<Creature> GetHeroList()
        {
            return _creatureDict.Select(x => x.Value)
                .Where(x => x.CreatureType == Define.CreatureType.Hero)
                .ToList();
        }
    }
}