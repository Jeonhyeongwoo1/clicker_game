using System;
using System.Collections.Generic;
using System.IO;
using Clicker.Controllers;
using Clicker.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Clicker.Manager
{
    [Serializable]
    public class ItemSaveData
    {
        public int dataId;
        public int dbId;
        public int count = 1;
        public int slotId;
        public int instanceId;
        public bool isEquipped;
    }

    [Serializable]
    public class QuestSaveData
    {
        public int dataId;
        public int questType;
        public int currentValue;
        public int achievementValue;
    }

    [Serializable]
    public class GameSaveData
    {
        public int Wood = 0;
        public int Mineral = 0;
        public int Meat = 0;
        public int Gold = 0;

        public int DbId = 0;
        public List<HeroSaveData> Heroes = new List<HeroSaveData>();
        public List<ItemSaveData> Items = new List<ItemSaveData>();
        public List<QuestSaveData> Quests = new List<QuestSaveData>();
    }

    [Serializable]
    public class HeroSaveData
    {
        public int DataId = 0;
        public int Level = 1;
        public int Exp = 0;
        public HeroOwningState OwningState = HeroOwningState.Unowned;
    }

    public enum HeroOwningState
    {
        Unowned,
        Owned,
        Picked,
    }
    
    public class GameManager
    {

        public long Dia;
        public long BattlePower;
        public long Gold;
        public int Wood;
        public int Mineral;
        public int Meat;
        
        public List<Creature> HeroList;

        public void InitGame()
        {
            
        }
        
        #region GameData
        public GameSaveData GameSaveData
        {
            get => _gameSaveSaveData;
            private set => _gameSaveSaveData = value;
        }
        
        private GameSaveData _gameSaveSaveData = new GameSaveData();
        
        #endregion

        #region SaveData

        public string Path { get { return Application.persistentDataPath + "/SaveData.json"; } }
        public void SaveGameData()
        {
            string jsonData = JsonConvert.SerializeObject(GameSaveData, Formatting.Indented);

            try
            {
                File.WriteAllText(Path, jsonData);
            }
            catch (Exception e)
            {
                LogUtils.LogError("Failed save game data :" + e.Message);
                return;
            }
        }

        public bool LoadGameData()
        {
            if (!File.Exists(Path))
            {
                return false;
            }
            
            string data = null;
            try
            {
                data = File.ReadAllText(Path);
            }
            catch (Exception e)
            {
                //Go to Title scene
                LogUtils.LogError("Failed save game data :" + e.Message);
                return false;
            }
            
            if (string.IsNullOrEmpty(data))
            {
                LogUtils.LogError("Failed load game data :" + Path);
                return false;
            }

            var gameData = JsonConvert.DeserializeObject<GameSaveData>(data);
            GameSaveData = gameData;

            return true;
        }
        
        #endregion

        public int GenerateDBId()
        {
            int id = GameSaveData.DbId;
            GameSaveData.DbId++;
            return id;
        }
        
        private Vector3Int GetNearbyPosition(Vector3 position, float range = 5)
        {
            const int count = 100;
            for (int i = 0; i < count; i++)
            {
                float x = Random.Range(-range, range);
                float y = Random.Range(-range, range);
                Vector3 targetPos = position + new Vector3(x, y);
                Vector3Int cellPos = Managers.Map.WorldToCell(targetPos);
                if (!Managers.Map.CanGo(cellPos.x, cellPos.y, null))
                {
                    continue;
                }

                return cellPos;
            }

            return Managers.Map.WorldToCell(position);
        }

        public void TeleportHeros(string name)
        {
            StageTranslation stageTranslation = Managers.Map.StageTranslation;
            bool isSuccess = stageTranslation.TryChangeStage(name);
            if (isSuccess)
            {
                Vector3 startPos = stageTranslation.GetWayPosition();
                foreach (Creature creature in Managers.Object.HeroSet)
                {
                    Vector3Int position = GetNearbyPosition(startPos);
                    Managers.Map.MoveToCell(position, creature.CellPosition, creature);
                }
                
                Managers.Object.HeroCamp.MoveToWaypointPosition(startPos);
            }
        }

    }
}