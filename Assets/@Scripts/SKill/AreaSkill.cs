using System;
using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.Skill
{
    public class AreaSkill : BaseSKill
    {
        protected virtual float Angle => 360f;
        
        protected SpellIndicator _spellIndicator;
        protected Vector3 _direction = Vector3.zero;
        protected RaycastHit2D[] _hit2Ds = new RaycastHit2D[20];
        
        protected void AddSpellIndicator()
        {
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
        
        public override void SetInfo(int id, Creature owner, Define.SkillType skillType,
            Action<BaseSKill> onCompleteSkillAction, Action<BaseSKill> onStopSkillAction)
        {
            base.SetInfo(id, owner, skillType, onCompleteSkillAction, onStopSkillAction);
            AddSpellIndicator();
        }
            
        protected override void UseSKill()
        {
            if (!_onwer.IsValid() || !_onwer.TargetObject.IsValid())
            {
                LogUtils.LogWarning("owner, target is valid");
                return;
            }
            
            _onwer.PlayAnimation(0, _skillData.AnimName, false).TimeScale = 1;
            _direction = (_onwer.TargetObject.transform.position - transform.position).normalized;
            _spellIndicator.Cancel();
            _spellIndicator.ConeFillAsync(_onwer.TargetObject, _skillData.AnimImpactDuration, Angle).Forget();
        }

        public override void StopSkill()
        {
            base.StopSkill();
            _spellIndicator.Cancel();
        }

        protected List<BaseObject> FindTargetAndTakeDamage(Vector3 origin, float radius, float angle, Vector3 direction,
            float distance, LayerMask layerMask)
        {
            int count = Physics2D.CircleCastNonAlloc(origin, radius, direction, _hit2Ds, distance, layerMask);
            if (count == 0)
            {
                return null;
            }

            List<BaseObject> result = new List<BaseObject>();
            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = _hit2Ds[i];
                if (hit.transform == transform)
                {
                    continue;
                }

                Vector3 toTarget = (hit.transform.position - origin).normalized;
                float dot = Vector3.Dot(direction, toTarget);
                bool isHit = dot > Mathf.Cos(angle * Mathf.Deg2Rad / 2);
                if (!isHit)
                {
                    continue;
                }

                if (hit.transform.TryGetComponent(out BaseObject baseObject))
                {
                    result.Add(baseObject);
                    // baseObject.TakeDamage(_onwer, _skillData);
                }
            }

            return result;
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