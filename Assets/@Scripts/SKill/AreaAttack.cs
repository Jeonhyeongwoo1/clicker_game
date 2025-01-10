using System;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace Clicker.Skill
{
    public class AreaAttack : BaseSKill
    {
        private SpellIndicator _spellIndicator;
        private Vector3 _direction = Vector3.zero;
        
        public override void SetInfo(int id)
        {
            base.SetInfo(id);

            _skillData = Managers.Data.SkillDataDict[id];
            
            var prefab = Managers.Resource.Instantiate(_skillData.PrefabLabel);
            if (!prefab.TryGetComponent(out SpellIndicator spellIndicator))
            {
                LogUtils.LogError("Failed to instantiate spellIndicator");
                return;
            }

            _spellIndicator = spellIndicator;
            _spellIndicator.SetInfo(transform, _skillData);
            _spellIndicator.gameObject.SetActive(false);
        }

        public override void UseSKill()
        {
            if (!_onwer.IsValid() || !_onwer.TargetObject.IsValid())
            {
                LogUtils.LogWarning("owner, target is valid");
                return;
            }

            _direction = (_onwer.TargetObject.transform.position - transform.position).normalized;
            _spellIndicator.Cancel();
            _spellIndicator.ConeFillAsync(_onwer.TargetObject, _skillData.AnimImpactDuration).Forget();
        }

        public override void StopSkill()
        {
            base.StopSkill();
            
            _spellIndicator.Cancel();
        }

        private RaycastHit2D[] _hit2Ds = new RaycastHit2D[20];
        private void Attack(Vector3 origin, float radius, float angle, Vector3 direction, float distance, LayerMask layerMask)
        {
            int count = Physics2D.CircleCastNonAlloc(origin, radius, direction, _hit2Ds, distance, layerMask);
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = _hit2Ds[i];
                Vector3 toTarget = (hit.transform.position - origin).normalized;
                float dot = Vector3.Dot(direction, toTarget);
                bool isHit = dot > Mathf.Cos(angle * Mathf.Deg2Rad / 2);
                if (!isHit)
                {
                    continue;
                }
                
                if (hit.transform.TryGetComponent(out BaseObject baseObject))
                {
                    baseObject.TakeDamage(_onwer, _skillData);
                }
            }
        }

        protected override void OnAnimationEvent(TrackEntry trackEntry, Event e)
        {
            base.OnAnimationEvent(trackEntry, e);

            if (trackEntry.Animation.Name.Contains(Define.AnimationName.skill_a))
            {
                Attack(transform.position, _skillData.SkillRange * _skillData.ScaleMultiplier, 120,
                    _direction, _skillData.SkillRange, LayerMask.GetMask("Hero"));
                _onwer.PlayAnimation(0, _skillData.AnimName, false);
                UseSKill();
            }
        }

        Vector2 RotateVector(Vector2 vector, float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }

        void OnDrawGizmos()
        {
            Vector2 origin = transform.position;
            Vector2 direction = _direction; // 현재 오브젝트의 오른쪽 방향
            float angle = 120f;
            float range = 5f;

            Gizmos.color = Color.yellow;

            // 콘의 범위 그리기
            float halfAngle = angle / 2f;
            Vector2 leftBoundary = RotateVector(direction, -halfAngle) * range;
            Vector2 rightBoundary = RotateVector(direction, halfAngle) * range;

            Gizmos.DrawLine(origin, origin + leftBoundary);
            Gizmos.DrawLine(origin, origin + rightBoundary);
            Gizmos.DrawWireSphere(origin, range);
        }
    }
}