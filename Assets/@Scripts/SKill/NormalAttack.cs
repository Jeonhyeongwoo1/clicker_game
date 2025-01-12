using Clicker.Manager;
using Clicker.Utils;

namespace Clicker.Skill
{
    public class NormalAttack : BaseSKill
    {
        protected override void OnAttackEvent()
        {
            if (!_onwer.IsValid() || !_onwer.TargetObject.IsValid())
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

            StopSkill();
        }

        protected override void UseSKill()
        {
            _onwer.PlayAnimation(0, _skillData.AnimName, false).TimeScale = 1;
        }
    }
}