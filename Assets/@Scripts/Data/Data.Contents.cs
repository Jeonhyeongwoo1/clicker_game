using System;
using System.Collections;
using System.Collections.Generic;
using Clicker.Manger;
using UnityEngine;

namespace Clicker.ContentData
{
	namespace Data
	{
		#region CreatureData
		[Serializable]
		public class CreatureData
		{
			public int DataId;
			public string DescriptionTextID;
			public string PrefabLabel;
			public float ColliderOffsetX;
			public float ColliderOffstY;
			public float ColliderRadius;
			public float Mass;
			public float MaxHp;
			public float MaxHpBonus;
			public float Atk;
			public float AtkRange;
			public float AtkBonus;
			public float Def;
			public float MoveSpeed;
			public float TotalExp;
			public float HpRate;
			public float AtkRate;
			public float DefRate;
			public float MoveSpeedRate;
			public string SkeletonDataID;
			public string AnimatorName;
			public List<int> SkillIdList = new List<int>();
			public int DropItemId;
		}

		[Serializable]
		public class CreatureDataLoader : ILoader<int, CreatureData>
		{
			public List<CreatureData> creatures = new List<CreatureData>();

			public Dictionary<int, CreatureData> MakeDict()
			{
				Dictionary<int, CreatureData> dict = new Dictionary<int, CreatureData>();
				foreach (CreatureData creature in creatures)
					dict.Add(creature.DataId, creature);
				return dict;
			}
		}
		#endregion
		
		#region envData

		[Serializable]
		public class EnvData
		{
			public int DataId;
			public string DescriptionTextID;
			public string PrefabLabel;
			public float MaxHp;
			public int ResourceAmount;
			public float RegenTime;
			public List<String> SkeletonDataIDs = new List<String>();
			public int DropItemId;
		}

		[Serializable]
		public class EnvDataLoader : ILoader<int, EnvData>
		{
			public List<EnvData> envs = new();

			public Dictionary<int, EnvData> MakeDict()
			{
				Dictionary<int, EnvData> dict = new();
				foreach (EnvData envData in envs)
				{
					dict.Add(envData.DataId, envData);
				}
				
				return dict;
			}
		}

		#endregion
		
		#region TestData
		[Serializable]
		public class TestData
		{
			public int Level;
			public int Exp;
			public List<int> Skills;
			public float Speed;
			public string Name;
		}

		[Serializable]
		public class TestDataLoader : ILoader<int, TestData>
		{
			public List<TestData> tests = new List<TestData>();

			public Dictionary<int, TestData> MakeDict()
			{
				Dictionary<int, TestData> dict = new Dictionary<int, TestData>();
				foreach (TestData testData in tests)
					dict.Add(testData.Level, testData);

				return dict;
			}
		}
		#endregion
	}
}
