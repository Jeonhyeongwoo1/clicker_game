
using System;
using Clicker.ContentData;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Clicker.Controllers
{
    public class Env : BaseObject
    {
        public Define.EnvState EnvState
        {
            get => _envState;
            set
            {
                if (_envState != value)
                {
                    _envState = value;
                    UpdateAnimation();
                }
            }
        }
        
        private EnvData _envData;
        private Define.EnvState _envState;

        private static class AnimationName
        {
            public static string Idle = "idle";
            public static string Hit = "hit";
            public static string Dead = "dead";
        }
        
        public override void SetInfo(int id)
        {
            base.SetInfo(id);

            _envData = Managers.Data.EnvDataDict[id];

            _maxHp = _currentHp = _envData.MaxHp;
            string ranSpine = _envData.SkeletonDataIDs[Random.Range(0, _envData.SkeletonDataIDs.Count)];
            var dataAsset = Managers.Resource.Load<SkeletonDataAsset>(ranSpine);
            _animation.skeletonDataAsset = dataAsset;
            _animation.Initialize(true);
            EnvState = Define.EnvState.Idle;
            UpdateAnimation();
        }

        public override void TakeDamage(Creature attacker)
        {
            base.TakeDamage(attacker);
            
            EnvState = Define.EnvState.Hit;
        }

        public override void Dead()
        {
            base.Dead();
            
            EnvState = Define.EnvState.Dead;
        }

        protected override void UpdateAnimation()
        {
            base.UpdateAnimation();

            string animationName = AnimationName.Idle;
            switch (EnvState)
            {
                case Define.EnvState.Idle:
                    animationName = AnimationName.Idle;
                    PlayAnimation(0, animationName, true);
                    break;
                case Define.EnvState.Hit:
                    animationName = AnimationName.Hit;
                    PlayAnimation(0, animationName, false);
                    break;
                case Define.EnvState.Dead:
                    animationName = AnimationName.Dead;
                    PlayAnimation(0, animationName, false);
                    break;
            }
        }
    }
}