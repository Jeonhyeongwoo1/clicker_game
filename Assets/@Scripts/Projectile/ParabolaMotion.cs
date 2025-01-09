using System;
using Clicker.ContentData;
using Clicker.Controllers;
using UnityEngine;

namespace Clicker.Skill
{
    public class ParabolaMotion : BaseProjectileMotionComponent
    {
        private float _height = 10f;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _time;
        private Projectile _projectile;

        public override void Shoot(Creature owner, Projectile projectile, ProjectileData projectileData)
        {
            _time = 0f;
            _projectile = projectile;
            _startPosition = owner.transform.position;
            _endPosition = owner.TargetObject.transform.position;
            transform.position = owner.transform.position;
        }

        private Vector3 CalculatePosition(Vector3 startPosition, Vector3 endPosition, float time, float height)
        {
            Vector3 midPoint = Vector3.Lerp(startPosition, endPosition, time);
            float y = height * (1 - time) * time;
            midPoint.y += y;
            return midPoint;
        }
        
        private void Update()
        {
            if (_time > 1f)
            {
                _projectile.Destroy();
                return;
            }

            _time += Time.deltaTime;
            Vector3 currentPosition = CalculatePosition(_startPosition, _endPosition, _time, _height);
            transform.position = currentPosition;

            float time = Mathf.Clamp01(_time + 0.01f);
            Vector3 nextPosition = CalculatePosition(_startPosition, _endPosition, time, _height);
            Vector3 direction = (nextPosition - currentPosition).normalized;

            //180 : atan2 X축 양의 방향으로 계산, 화살촉은 x마이너스 방향이기 때문에
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += 180;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}