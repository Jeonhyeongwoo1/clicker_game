using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clicker.Utils
{
	public static class Define
	{
		public enum EScene
		{
			Unknown,
			TitleScene,
			GameScene,
		}

		public enum EUIEvent
		{
			Click,
			PointerDown,
			PointerUp,
			Drag,
		}

		public enum ESound
		{
			Bgm,
			Effect,
			Max,
		}
		
		public enum CreatureType
		{
			None,
			Hero,
			Monster
		}

		public enum MonsterState
		{
			None,
			Idle,
			Move,
			Attack,
			Dead
		}
	}
}