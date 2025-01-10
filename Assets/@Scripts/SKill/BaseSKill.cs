using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;

namespace Clicker.Skill
{
    public abstract class BaseSKill : BaseObject
    {
        protected SkillData _skillData;
        protected Creature _onwer;
        
        // private CancellationTokenSource _skillCts;
        
        public override bool Init(Define.EObjectType eObjectType)
        {
            if (base.Init(eObjectType) == false)
            {
                return false;
            }

            objectType = eObjectType;
            
            return true;
        }

        public override void SetInfo(int id)
        {
            base.SetInfo(id);

            _skillData = Managers.Data.SkillDataDict[id];
        }

        public void StartSkillProcess(Creature owner)
        {
            _onwer = owner;
            _onwer.PlayAnimation(0, _skillData.AnimName, false);
            _onwer.SkeletonAnimation.AnimationState.Event += OnAnimationEvent;
            _onwer.SkeletonAnimation.AnimationState.Complete += OnAnimationComplete;
            UseSKill();
        }

        public abstract void UseSKill();
        
        public virtual void StopSkill()
        {
            if (_onwer != null && _onwer.SkeletonAnimation.AnimationState != null)
            {
                _onwer.SkeletonAnimation.AnimationState.Event -= OnAnimationEvent;
                _onwer.SkeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
            }
            
            // if (_skillCts != null)
            // {
            //     _skillCts.Cancel();
            //     _skillData = null;
            // }

            // if (_onwer != null && _onwer.SkeletonAnimation.AnimationState != null)
            // {
            //     _onwer.SkeletonAnimation.AnimationState.Event -= OnAnimationEvent;
            //     _onwer.SkeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
            // }
        }
    }
}