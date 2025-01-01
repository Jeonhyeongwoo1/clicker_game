using Clicker.GameComponent;
using Clicker.Manager;
using Clicker.Utils;
using Scripts;

namespace Clicker.Controllers
{
    public class HeroController : Creature
    {
        private CreatureMoveComponent _moveComponent;

        protected static class AnimationName
        {
            public static string Idle = "idle";
            public static string Move = "move";
            public static string Attack = "attack";
            public static string Dead = "dead";
        }
        
        public override bool Init(Define.CreatureType creatureType)
        {
            _moveComponent = Util.GetOrAddComponent<CreatureMoveComponent>(gameObject);
            _moveComponent.Initialize(this);
            Managers.Camera.AddViewTarget(transform);
            
            return base.Init(creatureType);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InputHandler.onChangedUIEvent += OnChangeAnimationState;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InputHandler.onChangedUIEvent -= OnChangeAnimationState;
        }

        private void OnChangeAnimationState(Define.EUIEvent euiEvent)
        {
            switch (euiEvent)
            {
                case Define.EUIEvent.Click:
                    break;
                case Define.EUIEvent.PointerDown:
                    break;
                case Define.EUIEvent.PointerUp:
                    ChangeAnimation(AnimationName.Idle);
                    break;
                case Define.EUIEvent.Drag:
                    ChangeAnimation(AnimationName.Move);
                    break;
            }
        }
    }
}