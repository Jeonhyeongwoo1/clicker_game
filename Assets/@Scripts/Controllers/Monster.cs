using Clicker.GameComponent;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Controllers
{
    public class Monster : Creature
    {
        private AIComponent _aiComponent;
        
        public override bool Init(Define.ObjectType objectType)
        {
            base.Init(objectType);
            
            ChangeState(Define.CreatureState.Idle);
            _aiComponent = Util.GetOrAddComponent<AIComponent>(gameObject);
            _aiComponent.Initialize(this);
            return true;
        }

        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
            _aiComponent.StartAiProcess();
            _aiComponent.SetOriginPosition(spawnPosition);
        }

    }
}