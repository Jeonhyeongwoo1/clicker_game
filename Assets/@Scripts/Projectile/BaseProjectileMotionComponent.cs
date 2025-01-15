using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.Skill
{
    public abstract class BaseProjectileMotionComponent : MonoBehaviour
    {
        public virtual async UniTask DoMotion(Creature owner, Projectile projectile, ProjectileData projectileData)
        {
        }
    }
}