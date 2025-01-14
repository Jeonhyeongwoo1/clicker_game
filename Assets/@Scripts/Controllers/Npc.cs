using System.Collections;
using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.UI;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Controllers
{
    public class Npc : BaseObject
    {
        public Define.ENpcType NpcType => _npcType;
        
        private NpcData _npcData;
        private Define.ENpcType _npcType;
        private UI_NpcInteraction _uiNpcInteraction;
        
        public override void SetInfo(int id)
        {
            // base.SetInfo(id);

            _npcData = Managers.Data.NPCDataDict[id];
            _npcType = _npcData.NpcType;
            SetSpinAnimation(_npcData.SkeletonDataID);

            GameObject prefab = Managers.Resource.Instantiate(nameof(UI_NpcInteraction));
            if (!prefab.TryGetComponent(out UI_NpcInteraction uiNpcInteraction))
            {
                LogUtils.LogError("failed get ui npc intertaction");
                return;
            }
            
            _uiNpcInteraction = uiNpcInteraction;
        }

        public override void Spawn(Vector3 spawnPosition)
        {
            base.Spawn(spawnPosition);
            _uiNpcInteraction.SetInfo(this);
            gameObject.SetActive(true);
        }
    }
}