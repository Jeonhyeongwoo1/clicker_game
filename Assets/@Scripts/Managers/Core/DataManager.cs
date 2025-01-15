using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using UnityEngine;
using Clicker.ContentData;

namespace Clicker.Manger
{

	public interface ILoader<Key, Value>
	{
		Dictionary<Key, Value> MakeDict();
	}

	public class DataManager
	{
		public Dictionary<int, EnvData> EnvDataDict { get; set; }	
		public Dictionary<int, HeroData> HeroDataDict { get; set; }
		public Dictionary<int, MonsterData> MonsterDataDict { get; set; }
		public Dictionary<int, ProjectileData> ProjectileDataDict { get; set; }
		public Dictionary<int, SkillData> SkillDataDict { get; set; }
		public Dictionary<int, EffectData> EffectDataDict { get; set; }
		public Dictionary<int, AoEData> AoEDataDict { get; set; }
		public Dictionary<int, NpcData> NPCDataDict { get; set; }
		public Dictionary<int, HeroInfoData> HeroInfoDataDict { get; set; }
		public Dictionary<string, TextData> TextDataDict { get; set; }
		public Dictionary<int, ItemConsumableData> ItemConsumableDataDict { get; set; }
		public Dictionary<int, ItemEquipmentData> ItemEquipmentDataDict { get; set; }
		public Dictionary<int, DropTableData> DropTableDataDict { get; set; }
		public Dictionary<int, ItemData> ItemDataDict { get; set; }
		
		public void Init()
		{
			EnvDataDict = LoadJson<EnvDataLoader, int, EnvData>("EnvData").MakeDict();
			HeroDataDict = LoadJson<HeroDataLoader, int, HeroData>("HeroData").MakeDict();
			MonsterDataDict = LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDict();
			ProjectileDataDict = LoadJson<ProjectileDataLoader, int, ProjectileData>("ProjectileData").MakeDict();
			SkillDataDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
			EffectDataDict = LoadJson<EffectDataLoader, int, EffectData>("EffectData").MakeDict();	
			AoEDataDict = LoadJson<AoEDataLoader, int, AoEData>("AoEData").MakeDict();
			NPCDataDict = LoadJson<NpcDataLoader, int, NpcData>("NpcData").MakeDict();
			HeroInfoDataDict = LoadJson<HeroInfoDataLoader, int, HeroInfoData>("HeroInfoData").MakeDict();
			TextDataDict = LoadJson<TextDataLoader, string, TextData>("TextData").MakeDict();
			ItemConsumableDataDict = LoadJson<ItemConsumableDataLoader, int, ItemConsumableData>("Item_ConsumableData").MakeDict();
			ItemEquipmentDataDict = LoadJson<ItemEquipmentDataLoader, int, ItemEquipmentData>("Item_EquipmentData").MakeDict();
			DropTableDataDict = LoadJson<DropTableDataLoader, int, DropTableData>("DropTableData").MakeDict();

			ItemDataDict = new Dictionary<int, ItemData>();
			foreach (var (key, value) in ItemConsumableDataDict)
			{
				ItemDataDict[key] = value;
			}
			
			foreach (var (key, value) in ItemEquipmentDataDict)
			{
				ItemDataDict[key] = value;
			}
			
		}

		private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
		{
			TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
			return JsonConvert.DeserializeObject<Loader>(textAsset.text);
		}
	}
}
