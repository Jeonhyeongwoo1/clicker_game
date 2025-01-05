using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Skill
{
    public abstract class BaseProjectileMotionComponent : MonoBehaviour
    {
        public virtual void Shoot(Creature owner, Projectile projectile, ProjectileData projectileData)
        {
            if (owner == null)
            {
                projectile.Destroy();
                LogUtils.LogWarning("projectile is null");
                return;
            }
            
            transform.position = owner.transform.position;
            Vector3 targetPos = Vector2.zero;
            if (!owner.TargetObject.IsValid())
            {
                targetPos = owner.transform.right;
            }
            else
            {
                targetPos = owner.TargetObject.transform.position;
            }

            Vector2 direction = (targetPos - owner.transform.position).normalized;
            float speed = projectileData.ProjSpeed;
            projectile.Rigidbody2D.velocity = direction * speed;
        }
    }
}