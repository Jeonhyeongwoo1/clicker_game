using Clicker.Manager;
using Clicker.Utils;
using Spine;

namespace Clicker.Skill
{
    public class NormalAttack : BaseSKill
    {
        protected override void OnAnimationEvent(TrackEntry trackEntry, Event e)
        {
            base.OnAnimationEvent(trackEntry, e);

            if (trackEntry.Animation.Name.Contains(Define.AnimationName.Attack) ||
                trackEntry.Animation.Name.Contains(Define.AnimationName.Attack_a) ||
                trackEntry.Animation.Name.Contains(Define.AnimationName.Attack_b))
            {
                if (_onwer == null || _onwer.TargetObject == null)
                {
                    return;
                }

                if (_skillData.ProjectileId == 0)
                {
                    _onwer.TargetObject.TakeDamage(_onwer, _skillData);
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
                    
                    projectile.Shoot(_onwer, _skillData);
                }
            }
        }

        public override void UseSKill()
        {
            
        }
    }
}