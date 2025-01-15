using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using Clicker.Scene;
using Clicker.UI.SubItem;
using Clicker.Utils;
using UnityEngine;

namespace Clicker.Scene
{
    public class UI_GameScene : UI_Scene
    {
 
        [SerializeField] private UI_GoodsItem _diaItem;
        [SerializeField] private UI_GoodsItem _goldItem;
        [SerializeField] private UI_GoodsItem _battlePowerItem;

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            
            _diaItem.AddListener(OnClickGoodsItem);
            _goldItem.AddListener(OnClickGoodsItem);

            GameManager gameManager = Managers.Game;
            ResourceManager resourceManager = Managers.Resource;
            _goldItem.SetInfo(null, gameManager.Gold, Define.GoodsItemType.Gold);
            _diaItem.SetInfo(null, gameManager.Dia, Define.GoodsItemType.Dia);
            _battlePowerItem.SetInfo(null, gameManager.BattlePower, Define.GoodsItemType.BattlePower);

            return true;
        }

        private void OnClickGoodsItem(Define.GoodsItemType goodsItemType)
        {
            
        }
    }
}