using System;
using System.Collections.Generic;
using Clicker.ContentData;
using Clicker.Manager;
using Clicker.Utils;
using Scripts.Contents;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Clicker.ConfigData
{
    [CreateAssetMenu(fileName = "ItemDebugConfig", menuName = "Settings/ItemDebugConfig")]
    public class ItemDebugConfig : ScriptableObject
    {
        [SerializeField] private List<ItemData> itemDataList;
        
        [Button]
        public void AddItemInInventory(int id)
        {
            ItemData itemData = itemDataList[id];
            if (itemData == null)
            {
                Debug.LogError("failed item data" + id);
                return;
            }
            
            Managers.Inventory.AddItem(id);
        }
        
        [Button]
        public void UseConsumableItem(int id)
        {
            BaseItem item = Managers.Inventory.InventoryItemList.Find(v => v.DataId == id);
            Managers.Inventory.UseConsumableItem(item as ConsumableItem);
        }

        [Button]
        public void EquipItem(int id)
        {
            BaseItem item = Managers.Inventory.InventoryItemList.Find(v => v.DataId == id);
            Managers.Inventory.Equip(item as EquipItem);
        }
        
        public void AddItem()
        {
            foreach (var (key, value) in Managers.Data.ItemDataDict)
            {
                itemDataList.Add(value);
            }
        }
    }
}