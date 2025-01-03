using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using UnityEngine;
using Clicker.ContentData;
using Clicker.ContentData.Data;

namespace Clicker.Manger
{

	public interface ILoader<Key, Value>
	{
		Dictionary<Key, Value> MakeDict();
	}

	public class DataManager
	{
		public Dictionary<int, EnvData> EnvDataDict { get; set; }	
		public Dictionary<int, CreatureData> CreatureDataDict { get; set; }
		
		public void Init()
		{
			CreatureDataDict = LoadJson<CreatureDataLoader, int, CreatureData>("CreatureData").MakeDict();
			EnvDataDict = LoadJson<EnvDataLoader, int, EnvData>("EnvData").MakeDict();

		}

		private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
		{
			TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
			return JsonConvert.DeserializeObject<Loader>(textAsset.text);
		}
	}
}
