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
		public Dictionary<int, ContentData.Data.TestData> TestDic { get; private set; } = new();

		public void Init()
		{
			TestDic = LoadJson<ContentData.Data.TestDataLoader, int, ContentData.Data.TestData>("TestData").MakeDict();
		}

		private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
		{
			TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
			return JsonConvert.DeserializeObject<Loader>(textAsset.text);
		}
	}
}
