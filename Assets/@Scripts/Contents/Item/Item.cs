using Clicker.ContentData;
using Clicker.Manager;
using Clicker.Utils;

namespace Scripts.Contents
{
    public class BaseItem
    {
        public Define.EItemType ItemType => _itemData.Type;
        public Define.EItemSubType ItemSubType => _itemData.SubType;
        public int DataId => _itemData.DataId;
        public int Count => _itemSaveData.count;
        public ItemData ItemData => _itemData;
        
        protected ItemSaveData _itemSaveData;
        protected ItemData _itemData;

        public BaseItem(ItemData itemData)
        {
            _itemData = itemData;

            int id = Managers.Game.GenerateDBId();
            ItemSaveData saveData = new ItemSaveData
            {
                dataId = itemData.DataId,
                instanceId = id,
                dbId = id,
                isEquipped = false,
                count = 1,
                slotId = (int)Define.EItemType.Inventory
            };
            
            _itemSaveData = saveData;
            Managers.Game.GameSaveData.Items.Add(saveData);
            Managers.Game.SaveGameData();
        }

        public BaseItem(ItemData itemData, ItemSaveData saveData)
        {
            _itemData = itemData;
            _itemSaveData = saveData;
        }

        public bool IsMaxCount()
        {
            return _itemData.MaxStack == Count;
        }
    }

    public class EquipItem : BaseItem
    {
        public bool IsEquipped => _itemSaveData.isEquipped;

        public int Damage { get; private set; }
        public int Defence { get; private set; }
        public int Speed { get; private set; }
        
        public EquipItem(ItemData itemData) : base(itemData)
        {
            ItemEquipmentData data = (ItemEquipmentData)itemData;
            Damage = data.Damage;
            Defence = data.Defence;
            Speed = data.Speed;
        }

        public EquipItem(ItemData itemData, ItemSaveData saveData) : base(itemData, saveData)
        {
            ItemEquipmentData data = (ItemEquipmentData)itemData;
            Damage = data.Damage;
            Defence = data.Defence;
            Speed = data.Speed;
        }

        public void Equip()
        {
            ItemSaveData itemSaveData = Managers.Game.GameSaveData.Items.Find(v => v.instanceId == _itemSaveData.instanceId);
            itemSaveData.isEquipped = true;
            _itemSaveData = itemSaveData;
            Managers.Game.SaveGameData();
        }

        public void UnEquip()
        {
            ItemSaveData itemSaveData = Managers.Game.GameSaveData.Items.Find(v => v.instanceId == _itemSaveData.instanceId);
            itemSaveData.isEquipped = false;
            _itemSaveData = itemSaveData;
            Managers.Game.SaveGameData();
        }
    }

    public class ConsumableItem : BaseItem
    {
        public float Value { get; private set; }
        public float CoolTime { get; private set; }
        
        public ConsumableItem(ItemData itemData) : base(itemData)
        {
            ItemConsumableData data = (ItemConsumableData)itemData;
            CoolTime = data.CoolTime;
            Value = data.Value;
        }
        
        public ConsumableItem(ItemData itemData, ItemSaveData saveData) : base(itemData, saveData)
        {
            ItemConsumableData data = (ItemConsumableData)itemData;
            CoolTime = data.CoolTime;
            Value = data.Value;
        }

        public void AddConsumable()
        {
            ItemSaveData itemSaveData = Managers.Game.GameSaveData.Items.Find(v => v.instanceId == _itemSaveData.instanceId);
            itemSaveData.count++;
            _itemSaveData = itemSaveData;
            Managers.Game.SaveGameData();
        }

        public bool UseItem()
        {
            if (Count == 0)
            {
                return false;
            }
            
            ItemSaveData itemSaveData = Managers.Game.GameSaveData.Items.Find(v => v.instanceId == _itemSaveData.instanceId);
            itemSaveData.count--;
            _itemSaveData = itemSaveData;
            Managers.Game.SaveGameData();
            return true;
        }
    }
}