using Clicker.Controllers;
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
			GameObject map = Managers.Resource.Instantiate("BaseMap");
			Creature hero = Managers.Object.CreateCreature("Hero", Define.CreatureType.Hero);
			hero.Spawn(Vector3.zero);
			
			Creature monster = Managers.Object.CreateCreature("Monster", Define.CreatureType.Monster);
			monster.Spawn(new Vector3(10, 10));
		}

		public override void Clear()
		{

		}
	}
}