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
		
		public enum ObjectType
		{
			None,
			Hero,
			Monster,
			Env
		}

		public enum CreatureState
		{
			None,
			Idle,
			Move,
			Attack,
			Dead
		}

		public enum HeroMoveState
		{
			None,
			Idle,
			MoveToCreature,
			MoveToEnv,
			ReturnToHeroCamp,
			ForceMove
		}

		public enum ELayer
		{
			Default = 0,
			TransparentFX = 1,
			IgnoreRaycast = 2,
			Dummy1 = 3,
			Water = 4,
			UI = 5,
			Hero = 6,
			Monster = 7,
			GatheringResource = 8,
			Obstacle = 9,
			Projectile = 10,
		}
		
		public enum EnvState
		{
			None,
			Idle,
			Hit,
			Dead
		}
	}
}