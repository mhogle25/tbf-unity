using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BF2D.Combat;
using BF2D.Game.Actions;
using UnityEngine;

namespace BF2D.Game
{
    public class GameInfo : MonoBehaviour
    {
        //Singleton Reference
        public static GameInfo Instance { get { return GameInfo.instance; } }
        private static GameInfo instance;

        public float ClockSpeed { get { return this.clockSpeed; } }

        [Header("Global Game Settings")]
        [SerializeField] private float clockSpeed = 0.03125f;

        [Header("Data File Paths")]
        [SerializeField] private string savesPath = "Saves";
        [SerializeField] private string playersPath = "Players";
        [SerializeField] private string enemiesPath = "Enemies";
        [SerializeField] private string itemsPath = "Items";
        [SerializeField] private string equipmentsPath = "Equipments";
        [SerializeField] private string statusEffectsPath = "StatusEffects";
        [SerializeField] private string characterStatsActionsPath = "Gems";
         
        public List<CharacterStats> Players { get { return this.currentSave.Players; } }
        private SaveData currentSave = null;

        //String Caches (instantiate on get, modifiable discardable data classes)
        private readonly JsonStringCache<CharacterStats> players = new(5);
        private readonly JsonStringCache<CharacterStats> enemies = new(5);
        private readonly JsonStringCache<Item> items = new(20);

        //Object caches (no instantiation on get, single instance data classes)
        private readonly JsonObjectCache<Equipment> equipments = new(10);
        private readonly JsonObjectCache<StatusEffect> statusEffects = new(10);
        private readonly JsonObjectCache<CharacterStatsAction> characterStatsActions = new(10);

        private readonly List<ICache> externalCaches = new();

        //AssetCollections
        [Header("Asset Collections")]
        [SerializeField] private SpriteCollection iconCollection = null;
        [SerializeField] private AudioClipCollection soundEffectCollection = null;

        public CombatManager.InitializeInfo UnstageCombatInfo()
        { 
            if (this.queuedCombats.Count < 1)
            {
                return null;
            }
            return this.queuedCombats.Dequeue(); 
        }
        private readonly Queue<CombatManager.InitializeInfo> queuedCombats = new();

        private void Awake()
        {
            //Set this object not to destroy on loading new scenes
            DontDestroyOnLoad(this.gameObject);

            //Setup of Monobehaviour Singleton
            if (GameInfo.instance != this && GameInfo.instance != null)
            {
                Destroy(GameInfo.instance.gameObject);
            }

            GameInfo.instance = this;

            TEST_INITIALIZE();
        }

        private void TEST_INITIALIZE()
        {
            Debug.Log($"Streaming Assets Path: {Application.streamingAssetsPath}");
            Debug.Log($"Persistent Data Path: {Application.persistentDataPath}");

            this.currentSave = LoadSaveData("save1");
            List<CharacterStats> enemies = new()
            {
                InstantiateEnemy("en_lessergoblin")
            };

            this.queuedCombats.Enqueue(new CombatManager.InitializeInfo
            {
                players = this.Players,
                enemies = enemies
            });
        }

        public void SaveGame()
        {
            string newJSON = BF2D.Utilities.TextFile.SerializeObject(this.currentSave);
            StreamWriter writer = new StreamWriter(Path.Combine(Application.persistentDataPath, this.savesPath, this.currentSave.ID + ".json"), false);
            writer.WriteLine(newJSON);
            writer.Close();
        }

        public void ClearCaches()
        {
            foreach (ICache cache in this.externalCaches)
            {
                cache.Clear();
            }

            this.players.Clear();
            this.enemies.Clear();
            this.items.Clear();
            this.statusEffects.Clear();
            this.equipments.Clear();
            this.characterStatsActions.Clear();
        }

        public void RegisterCache(ICache cache)
        {
            this.externalCaches.Add(cache);
        }

        public void RemoveExternalCache(ICache cache)
        {
            this.externalCaches.Remove(cache);
        }

        public Sprite GetIcon(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetIcon] String was empty");
                return null;
            }
            return this.iconCollection[id];
        }

        public AudioClip GetSoundEffect(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetSoundEffect] String was empty");
                return null;
            }
            return this.soundEffectCollection[id];
        }

        public Item InstantiateItem(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiateItem] String was empty");
                return null;
            }
            this.items.Datapath = Path.Combine(Application.streamingAssetsPath, this.itemsPath);
            return this.items.Get(id);
        }

        public Equipment GetEquipment(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetEquipment] String was empty");
                return null;
            }
            this.equipments.Datapath = Path.Combine(Application.streamingAssetsPath, this.equipmentsPath);
            return this.equipments.Get(id);
        }

        public StatusEffect GetStatusEffect(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetStatusEffect] String was empty");
                return null;
            }

            this.statusEffects.Datapath = Path.Combine(Application.streamingAssetsPath, this.statusEffectsPath);
            return this.statusEffects.Get(id);
        }

        public CharacterStatsAction GetCharacterStatsAction(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetCharacterStatsAction] String was empty");
                return null;
            }

            this.characterStatsActions.Datapath = Path.Combine(Application.streamingAssetsPath, this.characterStatsActionsPath);
            return this.characterStatsActions.Get(id);
        }

        public CharacterStats InstantiateEnemy(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] String was empty");
                return null;
            }

            this.enemies.Datapath = Path.Combine(Application.streamingAssetsPath, this.enemiesPath);
            return this.enemies.Get(id);
        }

        public void NewPlayer(string playerID, string newName)
        {
            CharacterStats newPlayer = InstantiatePlayer(playerID);
            if (newPlayer == null)
            {
                Debug.LogWarning("[GameInfo:NewPlayer] InstantiatePlayer failed");
                return;
            }
            newPlayer.SetName(newName);
            this.currentSave.AddPlayer(newPlayer);
        }

        private CharacterStats InstantiatePlayer(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiatePlayer] String was empty");
                return null;
            }

            this.players.Datapath = Path.Combine(Application.streamingAssetsPath, this.playersPath);
            return this.players.Get(id);
        }

        private SaveData LoadSaveData(string saveID)
        {
            if (saveID == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] String was empty");
                return null;
            }

            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.persistentDataPath, this.savesPath, saveID + ".json"));
            if (content == string.Empty)
                return null;

            SaveData saveData = BF2D.Utilities.TextFile.DeserializeString<SaveData>(content);

            foreach (CharacterStats character in saveData.Players)
                character.Setup();

            return saveData;
        }
    }

}