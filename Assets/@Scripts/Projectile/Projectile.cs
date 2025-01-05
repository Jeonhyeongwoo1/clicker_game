using System;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Skill
{
    public class Projectile : BaseObject
    {
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        
        private ProjectileData _projectileData;
        private SpriteRenderer _spriteRenderer;
        private BaseProjectileMotionComponent _motionComponent;
        private readonly float _autoDestroyTime = 3f;
        private float _elapsed = 0f;
        private Creature _owner;

        public override bool Init(Define.ObjectType objectType)
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<CircleCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _objectType = objectType;
            return true;
        }

        public override void SetInfo(int id)
        {
            base.SetInfo(id);
            _projectileData = Managers.Data.ProjectileDataDict[id];
            _spriteRenderer.sprite = Managers.Resource.Load<Sprite>(_projectileData.ProjectileSpriteName);
            _spriteRenderer.sortingOrder = (int)Define.SortingOrder.Projectile;
            
            string namespaceString = "Clicker.Skill";
            _motionComponent =
                gameObject.AddComponent(Type.GetType($"{namespaceString}.{_projectileData.ComponentName}")) as
                    BaseProjectileMotionComponent;
        }

        public void Shoot(Creature owner)
        {
            if (_motionComponent == null)
            {
                LogUtils.LogError("motnion componet is null " + _projectileData.ComponentName);
                Destroy();
                return;
            }
            
            _owner = owner;
            _motionComponent.Shoot(owner, this, _projectileData);
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _autoDestroyTime)
            {
                _elapsed = 0f;
                Destroy();
            }
        }

        public void Destroy()
        {
            Managers.Object.Despawn(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(nameof(Monster)) || 
                other.CompareTag(nameof(Env)) || 
                other.CompareTag(nameof(Hero)))
            {
                if (other.gameObject != _owner.TargetObject.gameObject)
                {
                    return;
                }

                if (other.TryGetComponent(out BaseObject bo))
                {
                    bo.TakeDamage(_owner);
                    Destroy();
                }
            }
        }
    }
}