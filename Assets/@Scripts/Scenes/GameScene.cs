using System.Collections.Generic;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
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
			Managers.Map.CreateMap("BaseMap");
			Managers.Object.CreateObject<HeroCamp>(Define.EObjectType.HeroCamp, -1);

			for (int i = 0; i < 10; i++)
			{
				Vector3 spawnPos =new Vector3(Random.Range(-5, 5), Random.Range(5, -5));
				int id = 201001;
				if (i % 3 == 0)
				{
					id = 201002;
				}
				var hero = Managers.Object.CreateObject<Hero>(Define.EObjectType.Hero, id);
				
				hero.ExtraSize = 0;
				hero.Spawn(spawnPos);
			}
			
			Managers.Map.CreateBaseObjects();
			var monster = Managers.Object.CreateObject<Monster>(Define.EObjectType.Monster, 202006);
			monster.ExtraSize = 1;
			monster.Spawn(new Vector3(10,10));
			
			// var env = Managers.Object.CreateObject<Env>(Define.EObjectType.Env, 300001);
			// env.Spawn(new Vector3(10, 10));
		}

		public override void Clear()
		{

		}
	}
}