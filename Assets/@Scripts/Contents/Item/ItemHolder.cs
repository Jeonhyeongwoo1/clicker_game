using System;
using Clicker.ContentData;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Skill;
using Clicker.Utils;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Contents.ItemHolder
{
    public class ItemHolder : BaseObject
    {
        private ItemData _itemData;
        private SpriteRenderer _itemSpriteRenderer;
        private ParabolaMotion _parabolaMotion;
        
        public override bool Init(Define.EObjectType eObjectType)
        {
            objectType = eObjectType;
            _itemSpriteRenderer = Util.GetOrAddComponent<SpriteRenderer>(gameObject);
            _parabolaMotion = Util.GetOrAddComponent<ParabolaMotion>(gameObject);
            return true;
        }

        public override void SetInfo(int id)
        {
            base.SetInfo(id);
            _itemData = Managers.Data.ItemDataDict[id];
            _itemSpriteRenderer.sprite = Managers.Resource.Load<Sprite>("Object_Meat");
        }

        public override async void Spawn(Vector3 spawnPosition)
        {
            // Vector3Int spawnPos = Managers.Map.WorldToCell(spawnPosition);
            // _spawnPosition = spawnPos;
            transform.position = spawnPosition;
            float range = 1;
            Vector3 endPosition = transform.position + new Vector3(Random.Range(-range, range), Random.Range(-range, range));
            _parabolaMotion.transform.position = transform.position;
            _parabolaMotion.SetInfo(transform.position, endPosition,  1, 0, 5, false);
            
            await _parabolaMotion.MotionAsync();
            
            Sequence seq = DOTween.Sequence();
            seq.Append(_itemSpriteRenderer.DOFade(0, 0.5f));
            seq.OnComplete(()=> Managers.Object.Despawn(this));
        }
    }
}