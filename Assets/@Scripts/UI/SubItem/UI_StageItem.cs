using System;
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
        private Action<string> _onSelectStageAction;
        
        public void SetInfo(string stageName, Action<string> onSelectStageAction)
        {
            _stageNameText.text = stageName;
            _stageName = stageName;
            _onSelectStageAction = onSelectStageAction;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onSelectStageAction.Invoke(_stageName);
        }
    }
}