using System.Collections.Generic;
using System.Linq;
using Clicker.ContentData;
using Clicker.Controllers;
using Clicker.Utils;
using Scripts.Contents;

namespace Clicker.Manager
{
    public class InventoryManager
    {
        public IReadOnlyDictionary<int, BaseItem> EquippedItemDict => _equipItemDict;
        public IReadOnlyList<BaseItem> AllItemList => _allItemList;
        
        //모든 아이템
        private readonly List<BaseItem> _allItemList = new();
        //내 인벤토리에 있는 아이템(장비 + 소비)
        private readonly List<BaseItem> _inventoryItemList = new();
        //창고에 있는 아이템
        private readonly List<BaseItem> _warehouseItemList = new();
        //착용중인 장비 아이템
        private readonly Dictionary<int, BaseItem> _equipItemDict = new();
        
        public void UseConsumableItem(ConsumableItem consumableItem)
        {
            if (consumableItem.ItemSubType == Define.EItemSubType.HealthPotion)
            {
                BaseItem item = _inventoryItemList.Find(v => v == consumableItem);
                if (item == null)
                {
                    return;
                }
                
                ConsumableItem cItem = item as ConsumableItem;
                if (cItem == null)
                {
                    LogUtils.LogError("Failed get item");
                    return;
                }

                cItem.UseItem();
                
                foreach (Creature creature in Managers.Object.HeroSet)
                {
                    creature.Heal(cItem.ItemData as ItemConsumableData);
                }

                if (cItem.Count == 0)
                {
                    _inventoryItemList.Remove(cItem);
                }
            }
        }
        
        public void AddItem(int id)
        {
            ItemData itemData = Managers.Data.ItemDataDict[id];

            switch(itemData.ItemGroupType)
            {
                case Define.EItemGroupType.Equipment:
                    EquipItem item = new EquipItem(itemData); 
                    _inventoryItemList.Add(item);
                    _allItemList.Add(item);
                    break;
                case Define.EItemGroupType.Consumable:
                    BaseItem consumableItem = _inventoryItemList.Find(v => !v.IsMaxCount() && v.DataId == id);
                    if (consumableItem == null)
                    {
                        ConsumableItem newItem = new ConsumableItem(itemData);
                        _inventoryItemList.Add(newItem);
                        _allItemList.Add(newItem);
                        return;
                    }
                    
                    ConsumableItem cItem = consumableItem as ConsumableItem;
                    cItem.AddConsumable();
                    break;
            }
        }
        
        public void LoadSaveItem()
        {
            List<ItemSaveData> itemSaveDataList = Managers.Game.GameSaveData.Items;
            foreach (ItemSaveData itemSaveData in itemSaveDataList)
            {
                ItemData itemData = Managers.Data.ItemDataDict[itemSaveData.dataId];
                switch(itemData.ItemGroupType)
                {
                    case Define.EItemGroupType.Equipment:
                        EquipItem equipItem = new EquipItem(itemData, itemSaveData);
                        if (equipItem.IsEquipped)
                        {
                            _equipItemDict.Add((int)equipItem.ItemType, equipItem);
                        }
                        else
                        {
                            _inventoryItemList.Add(equipItem);
                        }
                        
                        break;
                    case Define.EItemGroupType.Consumable:
                        
                        break;
                }
            }

            SetEquipStat();
        }

        private void SetEquipStat()
        {
            if (_equipItemDict.Count > 0)
            {
                List<EquipItem> list = _equipItemDict.Values.OfType<EquipItem>().ToList();
                foreach (Creature creature in Managers.Object.HeroSet)
                {
                    creature.SetEquipStat(list);
                }
            }            
        }
        
        public void Equip(EquipItem item)
        {
            UnEquip(item);
            _equipItemDict[(int)item.ItemType] = item;
            _inventoryItemList.Remove(item);
            item.Equip();
            SetEquipStat();
        }
        
        public void UnEquip(EquipItem item)
        {
            if (_equipItemDict.TryGetValue((int)item.ItemType, out BaseItem oldItem))
            {
                (oldItem as EquipItem).UnEquip();
                _equipItemDict.Remove((int)item.ItemType);
                _inventoryItemList.Add(oldItem);
                SetEquipStat();
            }
        }

    }
}