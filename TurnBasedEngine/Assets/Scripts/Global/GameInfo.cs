using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BF2D.Combat;
using BF2D.Game.Actions;
using UnityEngine;
using BF2D.Utilities;
using TMPro;

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

        [Header("Data File Managers")]
        [SerializeField] private ExternalFileManager savesFileManager = null;
        [SerializeField] private FileManager playersFileManager = null;
        [SerializeField] private FileManager enemiesFileManager = null;
        [SerializeField] private FileManager itemsFileManager = null;
        [SerializeField] private FileManager equipmentsFileManager = null;
        [SerializeField] private FileManager statusEffectsFileManager = null;
        [SerializeField] private FileManager characterStatsActionsFileManager = null;
        [SerializeField] private FileManager jobFileManager = null;

        public List<CharacterStats> Players { get { return this.currentSave.Players; } }
        private SaveData currentSave = null;

        //String Caches (instantiate on get, modifiable discardable data classes)
        private readonly JsonStringCache<CharacterStats> players = new(5);
        private readonly JsonStringCache<CharacterStats> enemies = new(5);
        private readonly JsonStringCache<Item> items = new(20);

        //Object caches (no instantiation on get, single instance data classes)
        private readonly JsonEntityCache<Equipment> equipments = new(10);
        private readonly JsonEntityCache<StatusEffect> statusEffects = new(10);
        private readonly JsonEntityCache<CharacterStatsAction> characterStatsActions = new(10);
        private readonly JsonEntityCache<Job> jobs = new(10);

        private readonly List<ICache> externalCaches = new();

        //AssetCollections
        [Header("Asset Collections")]
        [SerializeField] private SpriteCollection iconCollection = null;
        [SerializeField] private AudioClipCollection soundEffectCollection = null;

        [Header("Fonts")]
        [SerializeField] private TMP_FontAsset mainFont;

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
            this.savesFileManager.WriteToFile(newJSON, this.currentSave.ID);
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
            return this.items.Get(id, this.itemsFileManager);
        }

        public Equipment GetEquipment(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetEquipment] String was empty");
                return null;
            }
            return this.equipments.Get(id, this.equipmentsFileManager);
        }

        public StatusEffect GetStatusEffect(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetStatusEffect] String was empty");
                return null;
            }
            return this.statusEffects.Get(id, this.statusEffectsFileManager);
        }

        public CharacterStatsAction GetCharacterStatsAction(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetCharacterStatsAction] String was empty");
                return null;
            }
            return this.characterStatsActions.Get(id, this.characterStatsActionsFileManager);
        }

        public Job GetJob(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetJob] String was empty");
            }
            return this.jobs.Get(id, this.jobFileManager);
        }

        public CharacterStats InstantiateEnemy(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] String was empty");
                return null;
            }
            return this.enemies.Get(id, this.enemiesFileManager);
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

        public bool ValidText(string text, out List<char> invalidCharacters)
        {
            return this.mainFont.HasCharacters(text, out invalidCharacters);
        }

        private CharacterStats InstantiatePlayer(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiatePlayer] String was empty");
                return null;
            }
            return this.players.Get(id, this.playersFileManager);
        }

        private SaveData LoadSaveData(string saveID)
        {
            if (saveID == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] String was empty");
                return null;
            }

            string content = this.savesFileManager.LoadFile(saveID);
            if (content == string.Empty)
                return null;

            SaveData saveData = BF2D.Utilities.TextFile.DeserializeString<SaveData>(content);

            foreach (CharacterStats character in saveData.Players)
                character.Setup();

            return saveData;
        }

    }

}