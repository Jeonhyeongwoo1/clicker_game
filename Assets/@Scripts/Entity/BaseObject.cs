using Clicker.Utils;
using Spine.Unity;
using UnityEngine;

namespace Clicker.Entity
{
    public class BaseObject : MonoBehaviour
    {
        public Define.ObjectType ObjectType => _objectType;
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
        public CircleCollider2D Collider2D => _collider2D;
        
        [SerializeField] protected Rigidbody2D _rigidbody2D;
        [SerializeField] protected CircleCollider2D _collider2D;
        [SerializeField] protected SkeletonAnimation _animation;

        protected Define.ObjectType _objectType;
        
        public virtual bool Init(Define.ObjectType objectType)
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<CircleCollider2D>();
            _animation = GetComponent<SkeletonAnimation>();
            _objectType = objectType;
            return true;
        }
        
    }
}