using System;
using System.Collections.Generic;
using Clicker.ConfigData;
using Clicker.Controllers;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Clicker.Scene
{
	public class GameScene : BaseScene
	{
		[SerializeField] private ItemDebugConfig _itemDebugConfig;
		
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
			Managers.Map.CreateMap("BaseMap");
			var camp = Managers.Object.CreateObject<HeroCamp>(Define.EObjectType.HeroCamp, -1);
			var cellPos = Managers.Map.WorldToCell(new Vector3(-100, -66));
			camp.SetPosition(Managers.Map.CellToWorld(cellPos));

			UI_GameScene sceneUI = Managers.UI.ShowSceneUI<UI_GameScene>();
			sceneUI.GetComponent<Canvas>().sortingOrder = 1;
			
			for (int i = 0; i < 1; i++)
			{
				Vector3 spawnPos =new Vector3(Random.Range(-5, 5), Random.Range(5, -5));
				int id = 201001;
				var hero = Managers.Object.CreateObject<Hero>(Define.EObjectType.Hero, id);
				
				hero.ExtraSize = 0;
				hero.Spawn(spawnPos + Managers.Map.CellToWorld(cellPos));
			}
			
			// var monster = Managers.Object.CreateObject<Monster>(Define.EObjectType.Monster, 202005);
			// monster.ExtraSize = 1;
			// monster.Spawn(new Vector3(5,5));
			
			// var env = Managers.Object.CreateObject<Env>(Define.EObjectType.Env, 300001);
			// env.Spawn(new Vector3(10, 10));
			
			_itemDebugConfig.AddItem();
		}

		public override void Clear()
		{

		}
	}
}