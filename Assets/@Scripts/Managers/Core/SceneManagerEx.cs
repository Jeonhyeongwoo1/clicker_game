using System.Collections;
using System.Collections.Generic;
using Clicker.Scene;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Clicker.Manager
{
	public class SceneManagerEx
	{
		public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

		public void LoadScene(Define.EScene type)
		{
			//Managers.Clear();
			SceneManager.LoadScene(GetSceneName(type));
		}

		private string GetSceneName(Define.EScene type)
		{
			string name = System.Enum.GetName(typeof(Define.EScene), type);
			return name;
		}

		public void Clear()
		{
			//CurrentScene.Clear();
		}
	}
}