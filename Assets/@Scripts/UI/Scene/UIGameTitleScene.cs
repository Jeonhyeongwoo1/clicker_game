using Clicker.Manager;
using Clicker.Scene;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clicker.Scene
{
	public class UIGameTitleScene : UI_Scene
	{
		public override bool Init()
		{
			if (base.Init() == false)
				return false;
			
			StartLoadAssets();

			return true;
		}

		void StartLoadAssets()
		{
			Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
			{
				Debug.Log($"{key} {count}/{totalCount}");

				if (count == totalCount)
				{
					Managers.Data.Init();
					bool isLoadedGameData = Managers.Game.LoadGameData();
					if (!isLoadedGameData)
					{
						Managers.Game.SaveGameData();
					}
					
					Managers.Game.InitGame();
					Managers.Scene.LoadScene(Define.EScene.LoginScene);
				}
			});
		}
	}
}