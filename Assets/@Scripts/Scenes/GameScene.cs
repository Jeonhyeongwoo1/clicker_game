using Clicker.Controllers;
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

			Managers.Game.CreateHeroTracker();

			float radius = 1.5f;
			int count = 1;
			for (int i = 0; i < count; i++)
			{
				float angle = i * Mathf.PI * 2 / count;
				float x = Mathf.Cos(angle) * radius;
				float y = Mathf.Sin(angle) * radius;

				Vector3 spawnPos = i == 0 ? Vector3.zero : new Vector3(x, y);
				var hero = Managers.Object.CreateCreature<Hero>(Define.ObjectType.Hero, 201001);
				hero.Spawn(spawnPos);
			}

			var monster = Managers.Object.CreateCreature<Monster>(Define.ObjectType.Monster, 202005);
			monster.Spawn(new Vector3(10, 10));
		}

		public override void Clear()
		{

		}
	}
}