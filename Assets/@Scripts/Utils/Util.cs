using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Clicker.Utils
{
	public static class Util
	{
		public static T GetOrAddComponent<T>(GameObject go) where T : Component
		{
			T component = go.GetComponent<T>();
			if (component == null)
				component = go.AddComponent<T>();

			return component;
		}

		public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
		{
			Transform transform = FindChild<Transform>(go, name, recursive);
			if (transform == null)
				return null;

			return transform.gameObject;
		}

		public static T FindChild<T>(GameObject go, string name = null, bool recursive = false)
			where T : UnityEngine.Object
		{
			if (go == null)
				return null;

			if (recursive == false)
			{
				for (int i = 0; i < go.transform.childCount; i++)
				{
					Transform transform = go.transform.GetChild(i);
					if (string.IsNullOrEmpty(name) || transform.name == name)
					{
						T component = transform.GetComponent<T>();
						if (component != null)
							return component;
					}
				}
			}
			else
			{
				foreach (T component in go.GetComponentsInChildren<T>())
				{
					if (string.IsNullOrEmpty(name) || component.name == name)
						return component;
				}
			}

			return null;
		}

		public static void SafeAddButtonListener(this Button button, UnityAction action)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(action);
		}

		public static Color HexToColor(string color)
		{
			if (color.Contains("#") == false)
				color = $"#{color}";

			ColorUtility.TryParseHtmlString(color, out Color parsedColor);

			return parsedColor;
		}
		
		public static T ParseEnum<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static void SafeCancelToken(ref CancellationTokenSource cts)
		{
			if (cts != null)
			{
				cts.Cancel();
				cts = null;
			}
		}

		public static void SafeAllocateToken(ref CancellationTokenSource cts)
		{
			if (cts != null)
			{
				cts.Cancel();
				cts = null;
			}

			cts = new CancellationTokenSource();
		}
		
		public static float GetEffectRadius(Define.EEffectSize size)
		{
			switch (size)
			{
				case Define.EEffectSize.CircleSmall:
					return Define.EFFECT_SMALL_RADIUS;
				case Define.EEffectSize.CircleNormal:
					return Define.EFFECT_NORMAL_RADIUS;
				case Define.EEffectSize.CircleBig:
					return Define.EFFECT_BIG_RADIUS;
				case Define.EEffectSize.ConeSmall:
					return Define.EFFECT_SMALL_RADIUS * 2f;
				case Define.EEffectSize.ConeNormal:
					return Define.EFFECT_NORMAL_RADIUS * 2f;
				case Define.EEffectSize.ConeBig:
					return Define.EFFECT_BIG_RADIUS * 2f;
				default:
					return Define.EFFECT_SMALL_RADIUS;
			}
		}

		public static IPAddress GetIpv4Address(string hostAddress)
		{
			IPAddress[] ipAddr = Dns.GetHostAddresses(hostAddress);

			if (ipAddr.Length == 0)
			{
				LogUtils.LogError("Auth server DNS failed");
				return null;
			}
			
			foreach (IPAddress ipAddress in ipAddr)
			{
				if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
				{
					return ipAddress;
				}
			}
			
			LogUtils.LogError("Auth server DNS failed");
			return null;
		}
	}
}
