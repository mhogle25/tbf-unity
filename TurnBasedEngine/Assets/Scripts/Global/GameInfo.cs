using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BF2D.Combat;
using BF2D.Game.Actions;
using UnityEngine;
using BF2D.Utilities;
using TMPro;
using BF2D.Enums;
using System;

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
        [SerializeField] private ExternalFileManager saveFilesManager = null;
        [SerializeField] private ExternalFileManager keyboardControlsConfigFilesManager = null;
        [SerializeField] private ExternalFileManager gamepadControlsConfigFilesManager = null;
        [SerializeField] private FileManager playersFileManager = null;
        [SerializeField] private FileManager enemiesFileManager = null;
        [SerializeField] private FileManager itemsFileManager = null;
        [SerializeField] private FileManager equipmentsFileManager = null;
        [SerializeField] private FileManager statusEffectsFileManager = null;
        [SerializeField] private FileManager characterStatsActionsFileManager = null;
        [SerializeField] private FileManager jobsFileManager = null;

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
            LoadControlsConfig(InputController.Keyboard, "keyboard_default");
            LoadControlsConfig(InputController.Gamepad, "gamepad_default");

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

        #region Public Methods
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

        public void NewGame(string saveID, string playerPrefabID, string playerName)
        {
            SaveData newGame = new SaveData
            {
                ID = saveID
            };
            this.currentSave = newGame;
            NewPlayer(playerPrefabID, playerName);
        }

        public void SaveGame()
        {
            SaveGameAs(this.currentSave.ID);
        }

        public void SaveGameAs(string id)
        {
            this.currentSave.ID = id;
            string newJSON = BF2D.Utilities.TextFile.SerializeObject(this.currentSave);
            this.saveFilesManager.WriteToFile(newJSON, id);
        }

        public void LoadGame(string id)
        {
            this.currentSave = LoadSaveData(id);
        }

        public void NewControlsConfig(InputController controllerType)
        {
            InputManager.ResetConfig(controllerType);
        }

        public void SaveControlsConfig(InputController controllerType)
        {
            string id = controllerType switch
            {
                InputController.Keyboard => InputManager.KeyboardID,
                InputController.Gamepad => InputManager.GamepadID,
                _ => throw new ArgumentException("[GameInfo:SaveControlsConfig] InputController enum value was invalid")
            };
            SaveControlsConfigAs(controllerType, id);
        }

        public void SaveControlsConfigAs(InputController controllerType, string id)
        {
            string newJSON = InputManager.SerializeConfig(controllerType);

            switch (controllerType)
            {
                case InputController.Keyboard:
                    InputManager.KeyboardID = id;
                    this.keyboardControlsConfigFilesManager.WriteToFile(newJSON, id);
                    break;
                case InputController.Gamepad:
                    InputManager.GamepadID = id;
                    this.gamepadControlsConfigFilesManager.WriteToFile(newJSON, id);
                    break;
                default:
                    Debug.LogError("[GameInfo:SaveControlsConfigAs] InputController enum value was invalid");
                    break;
            }
        }

        public void LoadControlsConfig(InputController controllerType, string id)
        {
            string newJSON = controllerType switch
            {
                InputController.Keyboard => this.keyboardControlsConfigFilesManager.LoadFile(id),
                InputController.Gamepad => this.gamepadControlsConfigFilesManager.LoadFile(id),
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(newJSON))
            {
                Debug.LogError("[GameInfo:LoadControlsConfig] Fetch failed");
                return;
            }

            InputManager.DeserializeConfig(controllerType, newJSON);
        }

        public Sprite GetIcon(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:GetIcon] ID was invalid.");
                return null;
            }
            return this.iconCollection[id];
        }

        public AudioClip GetSoundEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:GetSoundEffect] ID was invalid.");
                return null;
            }
            return this.soundEffectCollection[id];
        }

        public Item InstantiateItem(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:InstantiateItem] ID was invalid.");
                return null;
            }
            return this.items.Get(id, this.itemsFileManager);
        }

        public Equipment GetEquipment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:GetEquipment] ID was invalid.");
                return null;
            }
            return this.equipments.Get(id, this.equipmentsFileManager);
        }

        public StatusEffect GetStatusEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:GetStatusEffect] ID was invalid.");
                return null;
            }
            return this.statusEffects.Get(id, this.statusEffectsFileManager);
        }

        public CharacterStatsAction GetCharacterStatsAction(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:GetCharacterStatsAction] ID was invalid.");
                return null;
            }
            return this.characterStatsActions.Get(id, this.characterStatsActionsFileManager);
        }

        public Job GetJob(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:GetJob] ID was invalid.");
            }
            return this.jobs.Get(id, this.jobsFileManager);
        }

        public CharacterStats InstantiateEnemy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] ID was invalid.");
                return null;
            }
            return this.enemies.Get(id, this.enemiesFileManager);
        }

        public void NewPlayer(string playerID, string newName)
        {
            CharacterStats newPlayer = InstantiatePlayer(playerID);
            if (newPlayer is null)
            {
                Debug.LogWarning("[GameInfo:NewPlayer] InstantiatePlayer failed");
                return;
            }
            newPlayer.SetName(newName);
            this.currentSave.AddPlayer(newPlayer);
        }

        public CombatManager.InitializeInfo UnstageCombatInfo()
        {
            if (this.queuedCombats.Count < 1)
            {
                return null;
            }
            return this.queuedCombats.Dequeue();
        }
        #endregion

        #region Private Utilities
        private CharacterStats InstantiatePlayer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:InstantiatePlayer] ID was invalid.");
                return null;
            }
            return this.players.Get(id, this.playersFileManager);
        }

        private SaveData LoadSaveData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] ID was invalid.");
                return null;
            }

            string content = this.saveFilesManager.LoadFile(id);
            if (string.IsNullOrEmpty(content))
                return null;

            SaveData saveData = BF2D.Utilities.TextFile.DeserializeString<SaveData>(content);

            foreach (CharacterStats character in saveData.Players)
                character.Setup();

            return saveData;
        }
        #endregion
    }

}