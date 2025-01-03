using Clicker.GameComponent;
using Clicker.Manager;
using Clicker.Utils;
using Scripts;
using UnityEngine;

namespace Clicker.Controllers
{
    public class Hero : Creature
    {
        private CreatureMoveComponent _moveComponent;
        private AIComponent _aiComponent;

        protected static class AnimationName
        {
            public static string Idle = "idle";
            public static string Move = "move";
            public static string Attack = "attack";
            public static string Dead = "dead";
        }
        
        public override bool Init(Define.ObjectType objectType)
        {
            base.Init(objectType);
            _moveComponent = Util.GetOrAddComponent<CreatureMoveComponent>(gameObject);
            _moveComponent.Initialize(this, Managers.Game.HeroCamp.transform);
            _aiComponent = Util.GetOrAddComponent<AIComponent>(gameObject);
            _aiComponent.Initialize(this);
            
            return true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputHandler.onChangedUIEvent += OnChangedUIEvent;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InputHandler.onChangedUIEvent -= OnChangedUIEvent;
        }

        private void OnChangedUIEvent(Define.EUIEvent euiEvent)
        {
            switch (euiEvent)
            {
                case Define.EUIEvent.Click:
                    break;
                case Define.EUIEvent.PointerDown:
                    _moveComponent.SetIsPossibleMove(true);
                    _aiComponent.StopAIProcess();
                    break;
                case Define.EUIEvent.PointerUp:
                    ChangeAnimation(AnimationName.Idle);
                    ChangeState(Define.CreatureState.Idle);
                    _moveComponent.SetIsPossibleMove(false);
                    _aiComponent.SetOriginPosition(transform.position);
                    _aiComponent.StartAiProcess();
                    break;
                case Define.EUIEvent.Drag:
                    ChangeAnimation(AnimationName.Move);
                    break;
            }
        }
    }
}