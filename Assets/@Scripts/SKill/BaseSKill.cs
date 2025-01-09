using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Spine;
using UnityEngine;
using Event = Spine.Event;

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
            UseSKill();
        }

        public abstract void UseSKill();
        
        public virtual void StopSkill()
        {
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

        protected override void OnAnimationEvent(TrackEntry trackEntry, Event e)
        {
            base.OnAnimationEvent(trackEntry, e);

            if (trackEntry.Animation.Name.Contains(Define.AnimationName.Attack) ||
                trackEntry.Animation.Name.Contains(Define.AnimationName.Attack_a) ||
                trackEntry.Animation.Name.Contains(Define.AnimationName.Attack_b))
            {
                if (trackEntry.Animation.Name == Define.AnimationName.Attack_a)
                {
                    int a = 0;
                }
                
                if (_onwer == null || _onwer.TargetObject == null)
                {
                    return;
                }

                if (_skillData.ComponentName == null)
                {
                    _onwer.TargetObject.TakeDamage(_onwer);
                }
                else
                {
                    var projectile =
                        Managers.Object.CreateObject<Projectile>(Define.EObjectType.Projectile, _skillData.ProjectileId);
            
                    if (projectile == null)
                    {
                        LogUtils.LogError($"Failed get proejctile : {_skillData.ProjectileId}");
                        return;
                    }
                
                    projectile.Shoot(_onwer);
                }
            }
        }
    }
}