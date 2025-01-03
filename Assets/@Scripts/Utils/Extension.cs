using System.Collections.Generic;
using Clicker.Entity;
using Clicker.Manager;
using UnityEngine;

namespace Clicker.Utils
{
	public static class Extension
	{
		public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
		{
			return Util.GetOrAddComponent<T>(go);
		}

		public static bool IsValid(this GameObject go)
		{
			return go != null && go.activeSelf;
		}

		public static bool IsValid(this BaseObject go)
		{
			return go != null && go.isActiveAndEnabled;
		}

		public static void DestroyChilds(this GameObject go)
		{
			foreach (Transform child in go.transform)
				Managers.Resource.Destroy(child.gameObject);
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;

			while (n > 1)
			{
				n--;
				int k = UnityEngine.Random.Range(0, n + 1);
				(list[k], list[n]) = (list[n], list[k]); //swap
			}
		}
	}
}