using System;
using System.Threading;
using Clicker.ContentData;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Clicker.Entity
{
    public class SpellIndicator : BaseObject
    {
        private SpriteRenderer _spriteRenderer;
        private CancellationTokenSource _skillCts;
        private SkillData _skillData;
        
        private readonly int Angle = Shader.PropertyToID("_Angle");
        private readonly int Duration = Shader.PropertyToID("_Duration");

        public void SetInfo(Transform owner, SkillData skillData)
        {
            _skillData = skillData;
            _spriteRenderer = Util.FindChild<SpriteRenderer>(gameObject, "Cone");
            transform.localScale = Vector3.zero;
            transform.localScale = Vector3.one * (_skillData.ScaleMultiplier * _skillData.SkillRange);
            transform.localPosition = Vector3.zero;
            transform.SetParent(owner);
        }
        
        public async UniTask ConeFillAsync(BaseObject targetObject, float duration, float angle)
        {
            Util.SafeAllocateToken(ref _skillCts);
            _spriteRenderer.material.SetFloat(Duration, 0);
            _spriteRenderer.material.SetFloat(Angle, angle);
            
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            
            Vector3 direction = (transform.position - targetObject.transform.position).normalized;
            float a = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(a, Vector3.forward);
            
            float elapsed = 0;
            // Material material = ;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _spriteRenderer.material.SetFloat(Duration, elapsed / duration);

                try
                {
                    await UniTask.Yield(cancellationToken: _skillCts.Token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    LogUtils.LogError($"{nameof(ConeFillAsync)} , {e.Message}");
                    break;
                }
            }
            
            Cancel();
        }

        public void Cancel()
        {
            // Debug.Log("Cancel");
            Util.SafeCancelToken(ref _skillCts);
            _spriteRenderer.material.SetFloat(Duration, 0);
            gameObject.SetActive(false);
        }

    }
}