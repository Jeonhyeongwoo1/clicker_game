using System;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.UI.Popup;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Clicker.UI
{
    public class UI_NpcInteraction : UI_Base
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        
        private Npc _owner;
        private Canvas _canvas;
        private Vector3 _offset = new Vector3(0, 4, 0);
        
        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }

            _canvas = Util.GetOrAddComponent<Canvas>(gameObject);
            _canvas.worldCamera = Camera.main;
            _button.onClick.AddListener(OnClickNpcInteraction);
            return true;
        }

        private void OnClickNpcInteraction()
        {
            switch (_owner.NpcType)
            {
                case Define.ENpcType.None:
                    break;
                case Define.ENpcType.StartPosition:
                    break;
                case Define.ENpcType.Guild:
                    break;
                case Define.ENpcType.Portal:
                    break;
                case Define.ENpcType.Waypoint:
                    var popup = Managers.UI.ShowPopupUI<UI_WaypointPopup>();
                    popup.Init();
                    break;
                case Define.ENpcType.BlackSmith:
                    break;
                case Define.ENpcType.Training:
                    break;
                case Define.ENpcType.TreasureBox:
                    break;
                case Define.ENpcType.Dungeon:
                    break;
                case Define.ENpcType.Quest:
                    break;
                case Define.ENpcType.GoldStorage:
                    break;
                case Define.ENpcType.WoodStorage:
                    break;
                case Define.ENpcType.MineralStorage:
                    break;
                case Define.ENpcType.Exchange:
                    break;
                case Define.ENpcType.RuneStone:
                    break;
            }
        }

        public void Destroy()
        {
            Managers.Resource.Destroy(gameObject);
        }

        public void SetInfo(Npc owner)
        {
            _owner = owner;
            Vector3 ownerPos = owner.transform.position;
            transform.position = ownerPos + _offset;
        }
    }
}