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
			LoginScene,
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
			Projectile,
			Effect,
			Aoe,
			Npc,
			Item
		}
		
		public enum CreatureState
		{
			None,
			Idle,
			Move,
			Attack,
			Dead,
			Stun
		}

		public enum HeroMoveState
		{
			None,
			Idle,
			MoveToCreature,
			MoveToEnv,
			ReturnToHeroCamp,
			ForceMove,
			ForcePath,
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
		
		public enum CollisionType
		{
			None,
			Wall,
			SemiWall //영웅들을 지나갈 수 없고 herocamp만 가능
		}
		
		public enum SkillType
		{
			DefaultSkill,
			EnvSkill,
			SkillA,
			SkillB,
		}
		
		public enum EffectClearType
		{
			TimeOut,
			EndOfAirbone,
			ClearSkill
		}
		
		
		public enum EEffectClassName
		{
			Bleeding,
			Poison,
			Ignite,
			Heal,
			AttackBuff,
			MoveSpeedBuff,
			AttackSpeedBuff,
			LifeStealBuff,
			ReduceDmgBuff,
			ThornsBuff,
			Knockback,
			Airborne,
			PullEffect,
			Stun,
			Freeze,
			CleanDebuff,
		}
		
		public enum EEffectType
		{
			Buff,
			Debuff,
			CrowdControl,
		}
		
		public enum BuffAndDebuffType
		{
			MoveSpeedBuff,
			AttackSpeedBuff,
			LifeStealBuff
		}
		
		public enum EStatModType
		{
			PercentAdd,
			PercentMult,
			Add
		}
		
		public enum EEffectSize
		{
			CircleSmall,
			CircleNormal,
			CircleBig,
			ConeSmall,
			ConeNormal,
			ConeBig,
		}
		
		public enum PathFineResultType
		{
			None,
			Lerp,
			Success,
			Fail,
			FailToMove,
		}

		public enum ELangauge
		{
			KOR,
		}

		public enum ENpcType
		{
			None,
			StartPosition,
			Guild,
			Portal,
			Waypoint,
			BlackSmith,
			Training,
			TreasureBox,
			Dungeon,
			Quest,
			GoldStorage,
			WoodStorage,
			MineralStorage,
			Exchange,
			RuneStone,
		}
		
		public enum GoodsItemType
		{
			Dia,
			Gold,
			BattlePower,
		}
		
		public enum EItemGrade
		{
			None,
			Normal,
			Rare,
			Epic,
			Legendary
		}

		public enum EItemGroupType
		{
			None,
			Equipment,
			Consumable,
		}

		public enum EItemType
		{
			None,
			Weapon,
			Armor,
			Helmet,
			Gloves,
			Boots,
			MaxEquipment,
			
			Inventory = 100, 
			WareHouse = 200
		}
		public enum EItemSubType
		{
			None,

			Sword,
			Dagger,
			Bow,

			Helmet,
			Armor,
			Shield,
			Gloves,
			Shoes,

			EnchantWeapon,
			EnchantArmor,

			HealthPotion,
			ManaPotion,
		}
		
		public enum EQuestPeriodType
		{
			Once, // 단발성
			Daily,
			Weekly,
			Infinite, // 무한으로
		}

		public enum EQuestCondition
		{
			None,
			Level,
			ItemLevel,
		}

		public enum EQuestStateType
		{
			None,
			Waiting,
			OnGoing,
			Completed,
			Rewarded
		}

		public enum EQuestObjectiveType
		{
			KillMonster,
			EarnMeat,
			SpendMeat,
			EarnWood,
			SpendWood,
			EarnMineral,
			SpendMineral,
			EarnGold,
			SpendGold,
			UseItem,
			Survival,
			ClearDungeon
		}

		public enum EQuestRewardType
		{
			Hero,
			Gold,
			Mineral,
			Meat,
			Wood,
			Item,
		}
		
		public enum EBroadcastEventType
		{
			None,
			ChangeMeat,
			ChangeWood,
			ChangeMineral,
			ChangeGold,
			ChangeDia,
			ChangeMaterials,
			KillMonster,
			LevelUp,
			DungeonClear,
			ChangeInventory,
			ChangeCrew,
			QuestClear,
		}
		
		public enum ProviderType
		{
			None,
			Guest,
			Google,
			Facebook
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
			public static string skill_b = "skill_b";
		}

		public static class SortingLayers
		{
			public const int AOE = 1;
			public const int SPELL_INDICATOR = 200;
			public const int CREATURE = 300;
			public const int ENV = 300;
			public const int PROJECTILE = 310;
			public const int SKILL_EFFECT = 310;
			public const int DAMAGE_FONT = 410;
		}
		
		public const char MAP_TOOL_WALL = '0';
		public const char MAP_TOOL_NONE = '1';
		public const char MAP_TOOL_SEMI_WALL = '2';
			
		public const float EFFECT_SMALL_RADIUS = 2.5f;
		public const float EFFECT_NORMAL_RADIUS = 4.5f;
		public const float EFFECT_BIG_RADIUS = 5.5f;

		public const float GAME_DATA_SAVE_TIME = 3;
	}
}