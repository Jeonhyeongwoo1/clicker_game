using System.Collections;
using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Controllers
{
    public class Npc : BaseObject
    {
        private NpcData _npcData;
        private Define.ENpcType _npcType;
        
        public override void SetInfo(int id)
        {
            base.SetInfo(id);

            _npcData = Managers.Data.NPCDataDict[id];
        }
    }
}