using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Clicker.UI.SubItem
{
    public class UI_StageItem : UI_Base, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI _stageNameText;

        private string _stageName;
        
        public void SetInfo(string stageName)
        {
            _stageNameText.text = stageName;
            _stageName = stageName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Managers.Map.ChangeStage(_stageName);
        }
    }
}