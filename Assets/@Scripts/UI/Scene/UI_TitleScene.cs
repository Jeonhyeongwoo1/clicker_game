using Clicker.Manager;
using Clicker.Scene;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clicker.Scene
{
	public class UI_TitleScene : UI_Scene
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
					Managers.Scene.LoadScene(Define.EScene.GameScene);
					Managers.Data.Init();
				}
			});
		}
	}
}