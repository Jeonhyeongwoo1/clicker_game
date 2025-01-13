using System;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Manger;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Spine;
using UnityEngine;

namespace Clicker.Skill
{
    public abstract class BaseSKill : MonoBehaviour
    {
        public Define.SkillType SkillType => _skillType;
        public int Id => _id;
        
        protected SkillData _skillData;
        protected Creature _onwer;

        private Define.SkillType _skillType;
        private int _id;
        private Action<BaseSKill> _onStopSkillAction;
        private Action<BaseSKill> _onCompleteSkillAction;
        
        public virtual void SetInfo(int id, Creature owner, Define.SkillType skillType, Action<BaseSKill> onCompleteSkillAction, Action<BaseSKill> onStopSkillAction)
        {
            _id = id;
            _skillType = skillType;
            _onwer = owner;
            _skillData = Managers.Data.SkillDataDict[id];
            _onStopSkillAction = onStopSkillAction;
            _onCompleteSkillAction = onCompleteSkillAction;
            if (_onwer != null && _onwer.SkeletonAnimation != null)
            {
                _onwer.SkeletonAnimation.AnimationState.Event += OnAnimationEvent;
                _onwer.SkeletonAnimation.AnimationState.Complete += OnAnimationComplete;
            }
        }

        public void StartSkillProcess()
        {
            UseSKill();
        }

        private void OnDisable()
        {
            if (_onwer != null && _onwer.SkeletonAnimation.AnimationState != null)
            {
                _onwer.SkeletonAnimation.AnimationState.Event -= OnAnimationEvent;
                _onwer.SkeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
            }
        }
        
        public virtual void StopSkill()
        {
            _onStopSkillAction?.Invoke(this);
        }

        protected async UniTaskVoid CoolTimeAsync()
        {
            float coolTime = _skillData.CoolTime;
            
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(coolTime), cancelImmediately: true);
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                LogUtils.LogError($"error {nameof(CoolTimeAsync)} / {e.Message}");   
            }
            
            StopSkill();
        }
        
        protected virtual void OnAnimationComplete(TrackEntry trackEntry)
        {
            _onCompleteSkillAction?.Invoke(this);
            if (trackEntry.Animation.Name == _skillData.AnimName)
            {
                OnAttackEvent();
            }
        }

        protected abstract void UseSKill();
        protected virtual void OnAnimationEvent(TrackEntry trackEntry, Spine.Event e)
        {
        }
        
        protected virtual void OnAttackEvent()
        {
            CoolTimeAsync().Forget();
        }
    }
}