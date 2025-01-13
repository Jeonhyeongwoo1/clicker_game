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
		
		public void Init()
		{
			EnvDataDict = LoadJson<EnvDataLoader, int, EnvData>("EnvData").MakeDict();
			HeroDataDict = LoadJson<HeroDataLoader, int, HeroData>("HeroData").MakeDict();
			MonsterDataDict = LoadJson<MonsterDataLoader, int, MonsterData>("MonsterData").MakeDict();
			ProjectileDataDict = LoadJson<ProjectileDataLoader, int, ProjectileData>("ProjectileData").MakeDict();
			SkillDataDict = LoadJson<SkillDataLoader, int, SkillData>("SkillData").MakeDict();
			EffectDataDict = LoadJson<EffectDataLoader, int, EffectData>("EffectData").MakeDict();	
			AoEDataDict = LoadJson<AoEDataLoader, int, AoEData>("AoEData").MakeDict();
		}

		private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
		{
			TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
			return JsonConvert.DeserializeObject<Loader>(textAsset.text);
		}
	}
}
