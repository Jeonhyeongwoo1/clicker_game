using System;
using System.Threading;
using System.Threading.Tasks;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;

namespace Clicker.Skill
{
    public class NormalAttack : BaseSKill
    {
        public override void UseSKill()
        {
            _onwer.SkeletonAnimation.AnimationState.Event += OnAnimationEvent;
            _onwer.SkeletonAnimation.AnimationState.Complete += OnAnimationComplete;
        }

        public override void StopSkill()
        {
            if (_onwer != null && _onwer.SkeletonAnimation.AnimationState != null)
            {
                _onwer.SkeletonAnimation.AnimationState.Event -= OnAnimationEvent;
                _onwer.SkeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
            }
        }
    }
}