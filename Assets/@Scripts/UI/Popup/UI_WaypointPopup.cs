using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using Clicker.UI.SubItem;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker.UI.Popup
{
    public class UI_WaypointPopup : UI_Popup
    {
        [SerializeField] private ScrollRect _scrollRect;

        private List<UI_StageItem> _subItemList = new();
        
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            
            return true;
        }

        private void DestroySubItem()
        {
            if (_subItemList.Count == 0)
            {
                return;
            }

            foreach (UI_StageItem item in _subItemList)
            {
                Managers.Resource.Destroy(item.gameObject);
            }
        }

        public void SetInfo(List<Stage> stageList)
        {
            DestroySubItem();
            
            foreach (Stage stage in stageList)
            {
                var stageItem = Managers.UI.MakeSubItem<UI_StageItem>(_scrollRect.content);
                _subItemList.Add(stageItem);
                stageItem.SetInfo(stage.name, OnSelectStage);
            }
        }

        private void OnSelectStage(string stageName)
        {
            Managers.Game.TeleportHeros(stageName);
            ClosePopupUI();
        }
    }
}