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
        [SerializeField] private List<QuestData> _questList = new ();
        [SerializeField] private List<ItemData> itemDataList;

        public void AddQuest()
        {
            foreach (var (key, value) in Managers.Data.QuestDataDict)
            {
                _questList.Add(value);
            }
        }

        [Button]
        public void AssignQuest(int id)
        {
            QuestData questData = Managers.Data.QuestDataDict[id];
            Managers.Quest.AssignQuest(questData);
        }
        
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
            BaseItem item = Managers.Inventory.InventoryItemList[id];
            Managers.Inventory.UseConsumableItem(item as ConsumableItem);
        }

        [Button]
        public void EquipItem(int id)
        {
            BaseItem item = Managers.Inventory.InventoryItemList[id];
            Managers.Inventory.Equip(item as EquipItem);
        }
        
        public void AddItem()
        {
            foreach (var (key, value) in Managers.Data.ItemDataDict)
            {
                itemDataList.Add(value);
            }

            AddQuest();
        }
    }
}