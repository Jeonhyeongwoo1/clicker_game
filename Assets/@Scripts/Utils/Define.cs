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
		
		public enum EObjectType
		{
			None,
			Hero,
			Monster,
			Env,
			HeroCamp,
			Skill,
			Projectile
		}
		public enum ECreatureType
		{
			None,
			Hero,
			Monster,
			Npc,
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
		
		public enum SortingOrder
		{
			Projectile = 30
		}	
		
		public enum CollisionType
		{
			None,
			Wall,
			SemiWall //영웅들을 지나갈 수 없고 herocamp만 가능
		}
		
		public class AnimationName
		{
			public static string Idle = "idle";
			public static string Move = "move";
			public static string Attack_a = "attack_a";
			public static string Attack = "attack";
			public static string Attack_b = "attack_b";
			public static string Dead = "dead";
			public static string Hit = "hit";
			public static string skill_a = "skill_a";
		}

		public const char MAP_TOOL_WALL = '0';
		public const char MAP_TOOL_NONE = '1';
		public const char MAP_TOOL_SEMI_WALL = '2';
	}
}