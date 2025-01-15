using System;
using System.Runtime.Serialization;
using System.Threading;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Manager;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Clicker.Entity
{
    public class BaseObject : MonoBehaviour
    {    
        public Vector3Int CellPosition => _cellPosition;
        public int ExtraSize { get; set; }
        public float Radius => _collider2D.radius;
        public Define.EObjectType ObjectType => objectType;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public CircleCollider2D Collider2D => _collider2D;
        public SkeletonAnimation SkeletonAnimation => _animation;
        protected virtual int SortingOrder => Define.SortingLayers.CREATURE;
        
        [SerializeField] protected Rigidbody2D _rigidbody2D;
        [SerializeField] protected CircleCollider2D _collider2D;
        [SerializeField] protected SkeletonAnimation _animation;

        protected Define.EObjectType objectType;
        protected int _id;
        protected Vector3Int _spawnPosition;
        [SerializeField] protected Vector3Int _cellPosition;
        private CancellationTokenSource _moveCts;
        private HurtFlashEffect _hurtFlashEffect;
        
        public virtual bool Init(Define.EObjectType eObjectType)
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<CircleCollider2D>();
            _animation = GetComponent<SkeletonAnimation>();
            _hurtFlashEffect = Util.GetOrAddComponent<HurtFlashEffect>(gameObject);
            objectType = eObjectType;
            return true;
        }
        
        protected virtual void OnEnable()
        {
        }
 
        protected virtual void OnDisable()
        {
        }
        public virtual void SetInfo(int id)
        {
            _id = id;
        }

        public virtual void TakeDamage(Creature attacker, SkillData skillData)
        {
            _hurtFlashEffect.Flash();
        }
        
        public virtual void Spawn(Vector3 spawnPosition)
        {
            Vector3Int spawnPos = Managers.Map.WorldToCell(spawnPosition);
            Managers.Map.MoveToCell(spawnPos, Vector3Int.zero, this, true);
            _spawnPosition = spawnPos;
            // transform.position = spawnPosition;
        }

        public virtual void Dead()
        {
        }

        protected virtual void UpdateAnimation()
        {
        }
        
        public virtual void SetCellPosition(Vector3Int cellPos, Vector3 cellWorldPos, bool forceMove = false)
        {
            _cellPosition = cellPos;

            if (ObjectType == Define.EObjectType.Hero)
            {
                Debug.LogError($"{cellWorldPos} / {cellPos}");
            }
            // Debug.LogError($"{transform.GetInstanceID()} / {cellPos} / {cellWorldPos} / {forceMove}");
            if (forceMove)
            {
                transform.position = cellWorldPos;
            }
        }

        protected void SetSpinAnimation(string id)
        {
            if (id == null)
            {
                return;
            }
            
            string skeletonDataID = id;
            var dataAsset = Managers.Resource.Load<SkeletonDataAsset>(skeletonDataID);
            if (dataAsset == null)
            {
                LogUtils.LogError($"Failed get skieleton asset : {skeletonDataID}");
                return;
            }
            
            _animation.skeletonDataAsset = dataAsset;
            _animation.Initialize(true);
            
            // Spine SkeletonAnimation은 SpriteRenderer 를 사용하지 않고 MeshRenderer을 사용함
            // 그렇기떄문에 2D Sort Axis가 안먹히게 되는데 SortingGroup을 SpriteRenderer,MeshRenderer을 같이 계산함.
            SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
            sg.sortingOrder = SortingOrder;
            
            _animation.AnimationState.Event += OnAnimationEvent;
            _animation.AnimationState.Complete += OnAnimationComplete;
            
        }
        
        public TrackEntry PlayAnimation(int trackIndex, string animName, bool loop)
        {
            if (_animation == null)
                return null;

            TrackEntry entry = _animation.AnimationState.SetAnimation(trackIndex, animName, loop);
            //animation blending
            if (animName == Define.AnimationName.Dead)
                entry.MixDuration = 0;
            else
                entry.MixDuration = 0.2f;

            return entry;
        }
                
        protected virtual void OnAnimationComplete(TrackEntry trackEntry)
        {
        }

        protected virtual void OnAnimationEvent(TrackEntry trackEntry, Spine.Event e)
        {
        }

        public virtual float GetTick(Define.CreatureState creatureState)
        {
            switch (creatureState)
            {
                case Define.CreatureState.Idle:
                    return 0.1f;
                case Define.CreatureState.Move:
                    return 0.0f;
                case Define.CreatureState.Attack:
                    return 0.1f;
                case Define.CreatureState.Dead:
                    return 0.3f;
            }

            //default
            return 0.5f;
        }
        
        protected void StopMoveToCellPosition()
        {
            Util.SafeCancelToken(ref _moveCts);
        }
        
        public void SetFlip(bool leftLook)
        {
            _animation.skeleton.ScaleX = leftLook ? -1 : 1;
        }
    }
}