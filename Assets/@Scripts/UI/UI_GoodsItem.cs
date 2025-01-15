using System.Collections;
using System.Collections.Generic;
using Clicker.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Clicker.UI.SubItem
{
    public class UI_GoodsItem : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private Button _button;

        private Define.GoodsItemType _itemTypeType;
        
        public void SetInfo(Sprite sprite, long amount, Define.GoodsItemType itemType)
        {
            if (sprite != null)
            {
                _iconImage.sprite = sprite;
            }
            
            _amountText.text = amount.ToString();
            _itemTypeType = itemType;
        }

        public void AddListener(UnityAction<Define.GoodsItemType> listener)
        {
            if (_button == null)
            {
                return;
            }

            _button.SafeAddButtonListener(() => listener.Invoke(_itemTypeType));
        }
    }
}