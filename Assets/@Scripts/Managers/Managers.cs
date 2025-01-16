using Clicker.Manger;
using Clicker.Utils;
using TMPro.EditorUtilities;
using UnityEngine;

namespace Clicker.Manager
{
	public class Managers : MonoBehaviour
	{
		private static Managers s_instance;
		private static Managers Instance { get { Init(); return s_instance; } }

		#region Content

		private ObjectManager _object = new ObjectManager();
		private GameManager _game = new GameManager();
		private MapManager _map = new MapManager();
		private InventoryManager _inventory = new InventoryManager();

		public static ObjectManager Object => Instance?._object;
		public static GameManager Game => Instance?._game;
		public static MapManager Map => Instance?._map;
		public static InventoryManager Inventory => Instance?._inventory;
		
		#endregion
		
		#region Core
		private DataManager _data = new DataManager();
		private PoolManager _pool = new PoolManager();
		private ResourceManager _resource = new ResourceManager();
		private SceneManagerEx _scene = new SceneManagerEx();
		private SoundManager _sound = new SoundManager();
		private UIManager _ui = new UIManager();
		private QuestManager _quest = new QuestManager();

		public static DataManager Data => Instance?._data;
		public static PoolManager Pool => Instance?._pool;
		public static ResourceManager Resource => Instance?._resource;
		public static SceneManagerEx Scene => Instance?._scene;
		public static SoundManager Sound => Instance?._sound;
		public static UIManager UI => Instance?._ui;
		public static QuestManager Quest => Instance?._quest;
		#endregion
		
		#region Language
		public Define.ELangauge Langauge => _langauge;
		private Define.ELangauge _langauge;
		public string GetLangaugeText(string id)
		{
			switch (_langauge)
			{
				case Define.ELangauge.KOR:
					return Managers.Data.TextDataDict[id].KOR;
			}

			return "FAIL!!!!!!!!!!";
		}
		#endregion
		
		public static void Init()
		{
			if (s_instance == null)
			{
				GameObject go = GameObject.Find("@Managers");
				if (go == null)
				{
					go = new GameObject { name = "@Managers" };
					go.AddComponent<Managers>();
				}

				DontDestroyOnLoad(go);

				// 초기화
				s_instance = go.GetComponent<Managers>();
			}
		}

	}
}
