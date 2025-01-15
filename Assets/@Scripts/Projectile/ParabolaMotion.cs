using System;
using System.Threading;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.Skill
{
    public class ParabolaMotion : BaseProjectileMotionComponent
    {
        private float _correct = 0f;
        private float _height = 10f;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _time;
        private float _duration = 1f;
        private bool _wantToRotation;
        private CancellationTokenSource _cts;

        public void SetInfo(Vector3 startPosition, Vector3 endPosition, float duration, float correct, float height, bool wantToRotation = true)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _duration = duration;
            _correct = correct;
            _height = height;
            _time = 0f;
            _wantToRotation = wantToRotation;
        }
        
        public override async UniTask DoMotion(Creature owner, Projectile projectile, ProjectileData projectileData)
        {
            // _startPosition = owner.transform.position;
            // _endPosition = owner.TargetObject.transform.position;
            transform.position = owner.transform.position;
            await MotionAsync();
        }

        private Vector3 CalculatePosition(Vector3 startPosition, Vector3 endPosition, float time, float height)
        {
            Vector3 midPoint = Vector3.Lerp(startPosition, endPosition, time);
            float y = height * (1 - time) * time;
            midPoint.y += y;
            return midPoint;
        }

        public async UniTask MotionAsync()
        {
            Util.SafeAllocateToken(ref _cts);

            while (_time < _duration)
            {
                _time += Time.deltaTime;
                Vector3 currentPosition = CalculatePosition(_startPosition, _endPosition, _time, _height);
                transform.position = currentPosition;

                float time = Mathf.Clamp01(_time + 0.01f);
                Vector3 nextPosition = CalculatePosition(_startPosition, _endPosition, time, _height);
                Vector3 direction = (currentPosition - nextPosition).normalized;

                if (_wantToRotation)
                {
                    //180 : atan2 X축 양의 방향으로 계산, 화살촉은 x마이너스 방향이기 때문에
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    angle += _correct;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                try
                {
                    await UniTask.Yield();
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        private void OnEnable()
        {
            _time = 0f;
        }
    }
}