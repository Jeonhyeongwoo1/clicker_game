using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Clicker.Scene
{
	public class GameScene : BaseScene
	{
		public override bool Init()
		{
			if (base.Init() == false)
				return false;

			SceneType = Define.EScene.GameScene;

			// TODO
			Initialize();
			
			return true;
		}

		private void Initialize()
		{
			GameObject map = Managers.Resource.Instantiate("BaseMap");

			Managers.Object.CreateObject<HeroCamp>(Define.ObjectType.HeroCamp, -1);

			float radius = 1.5f;
			int count = 1;
			for (int i = 0; i < count; i++)
			{
				float angle = i * Mathf.PI * 2 / count;
				float x = Mathf.Cos(angle) * radius;
				float y = Mathf.Sin(angle) * radius;

				Vector3 spawnPos = i == 0 ? Vector3.zero : new Vector3(x + Random.Range(-5, 5), y + Random.Range(5, -5));
				var hero = Managers.Object.CreateObject<Hero>(Define.ObjectType.Hero, 201002);
				hero.Spawn(spawnPos);
			}

			var monster = Managers.Object.CreateObject<Monster>(Define.ObjectType.Monster, 202005);
			monster.Spawn(new Vector3(10, 10));
			
			var env = Managers.Object.CreateObject<Env>(Define.ObjectType.Env, 300001);
			env.Spawn(new Vector3(10, 10));
		}

		public override void Clear()
		{

		}
	}
}