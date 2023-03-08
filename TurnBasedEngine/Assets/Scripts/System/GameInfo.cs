using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BF2D.Game.Actions;
using UnityEngine;
using BF2D.Utilities;
using TMPro;
using BF2D.Enums;
using System;
using BF2D.UI;

namespace BF2D.Game
{
    public class GameInfo : MonoBehaviour
    {
        //Singleton Reference
        public static GameInfo Instance { get { return GameInfo.instance; } }
        private static GameInfo instance;

        public DialogTextboxControl SystemTextbox { get { return this.systemTextbox; } }

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

        public CharacterStats Leader { get { return this.currentSave.Leader; } }
        public IEnumerable<CharacterStats> ActivePlayers { get { return this.currentSave is null ? null : this.currentSave.ActivePlayers; } }
        public IEnumerable<CharacterStats> InactivePlayers { get { return this.currentSave is null ? null : this.currentSave.InactivePlayers; } }
        public bool SaveActive { get { return this.currentSave is not null; } }
        public int Currency { get { return this.currentSave.Currency; } set { this.currentSave.Currency = value; } }
        private SaveData currentSave = null;

        //String Caches (instantiate on get, modifiable discardable data classes)
        private readonly JsonStringCache<CharacterStats> playerTemplates = new(5);
        private readonly JsonStringCache<CharacterStats> enemyTemplates = new(5);
        private readonly JsonStringCache<Item> itemTemplates = new(20);

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

        [Header("Miscellaneous")]
        [SerializeField] private DialogTextboxControl systemTextbox = null;

        private readonly Queue<Combat.CombatManager.InitializeInfo> queuedCombats = new();

        private void Awake()
        {
            //Setup of Monobehaviour Singleton
            if (GameInfo.instance != this && GameInfo.instance != null)
            {
                Destroy(GameInfo.instance.gameObject);
            }

            GameInfo.instance = this;
        }

        #region Public Methods
        public void ClearCaches()
        {
            foreach (ICache cache in this.externalCaches)
            {
                cache.Clear();
            }

            this.playerTemplates.Clear();
            this.enemyTemplates.Clear();
            this.itemTemplates.Clear();
            this.statusEffects.Clear();
            this.equipments.Clear();
            this.characterStatsActions.Clear();

            Terminal.IO.LogQuiet("Caches cleared");
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
            SaveData newGame = new()
            {
                ID = saveID
            };
            this.currentSave = newGame;
            NewPlayer(playerPrefabID, playerName);
        }

        public void SaveGame()
        {
            if (this.currentSave is null)
            {
                Terminal.IO.LogWarning("[GameInfo:SaveGame] Save failed, there was no game loaded");
                return;
            }

            SaveGameAs(this.currentSave.ID);
        }

        public void SaveGameAs(string id)
        {
            if (this.currentSave is null)
            {
                Terminal.IO.LogWarning("[GameInfo:SaveGameAs] Save failed, there was no game loaded");
                return;
            }

            this.currentSave.ID = id;
            string newJSON = BF2D.Utilities.TextFile.SerializeObject(this.currentSave);
            this.saveFilesManager.WriteToFile(newJSON, id);
            Terminal.IO.LogQuiet($"Saved to file with ID '{id}'");
        }

        public void LoadGame(string id)
        {
            this.currentSave = LoadSaveData(id);
        }

        public void NewControlsConfig(InputController controllerType)
        {
            InputManager.Instance.ResetConfig(controllerType);
        }

        public void SaveControlsConfig(InputController controllerType)
        {
            string id = controllerType switch
            {
                InputController.Keyboard => InputManager.Instance.KeyboardID,
                InputController.Gamepad => InputManager.Instance.GamepadID,
                _ => throw new ArgumentException("[GameInfo:SaveControlsConfig] InputController enum value was invalid")
            };
            SaveControlsConfigAs(controllerType, id);
        }

        public void SaveControlsConfigAs(InputController controllerType, string id)
        {
            string newJSON = InputManager.Instance.SerializeConfig(controllerType);

            switch (controllerType)
            {
                case InputController.Keyboard:
                    InputManager.Instance.KeyboardID = id;
                    this.keyboardControlsConfigFilesManager.WriteToFile(newJSON, id);
                    break;
                case InputController.Gamepad:
                    InputManager.Instance.GamepadID = id;
                    this.gamepadControlsConfigFilesManager.WriteToFile(newJSON, id);
                    break;
                default:
                    Terminal.IO.LogError("[GameInfo:SaveControlsConfigAs] InputController enum value was invalid");
                    break;
            }

            Terminal.IO.LogQuiet($"{controllerType} config saved to file with ID '{id}'");
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
                Terminal.IO.LogError("[GameInfo:LoadControlsConfig] Fetch failed");
                return;
            }

            InputManager.Instance.DeserializeConfig(controllerType, newJSON);

            Terminal.IO.LogQuiet($"{controllerType} config with id '{id}' was loaded");
        }

        public Sprite GetIcon(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:GetIcon] ID '{id}' was invalid");
                return null;
            }
            return this.iconCollection[id];
        }

        public AudioClip GetSoundEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:GetSoundEffect] ID '{id}' was invalid");
                return null;
            }
            return this.soundEffectCollection[id];
        }

        public Item InstantiateItem(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:InstantiateItem] ID '{id}' was invalid");
                return null;
            }
            return this.itemTemplates.Get(id, this.itemsFileManager);
        }

        public Equipment GetEquipment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:GetEquipment] ID '{id}' was invalid");
                return null;
            }
            return this.equipments.Get(id, this.equipmentsFileManager);
        }

        public StatusEffect GetStatusEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:GetStatusEffect] ID '{id}' was invalid");
                return null;
            }
            return this.statusEffects.Get(id, this.statusEffectsFileManager);
        }

        public CharacterStatsAction GetCharacterStatsAction(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:GetCharacterStatsAction] ID '{id}' was invalid");
                return null;
            }
            return this.characterStatsActions.Get(id, this.characterStatsActionsFileManager);
        }

        public Job GetJob(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:GetJob] ID '{id}' was invalid");
            }
            return this.jobs.Get(id, this.jobsFileManager);
        }

        public CharacterStats InstantiateEnemy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:InstantiateEnemy] ID '{id}' was invalid");
                return null;
            }
            CharacterStats enemy = this.enemyTemplates.Get(id, this.enemiesFileManager);
            if (enemy is null)
                return null;
            return enemy.Setup();
        }

        public void NewPlayer(string playerID, string newName)
        {
            CharacterStats newPlayer = InstantiatePlayer(playerID);
            if (newPlayer is null)
            {
                Terminal.IO.LogWarning("[GameInfo:NewPlayer] InstantiatePlayer failed");
                return;
            }
            newPlayer.SetName(newName);
            this.currentSave.AddPlayer(newPlayer);
        }

        public void StageCombatInfo(Combat.CombatManager.InitializeInfo info)
        {
            this.queuedCombats.Enqueue(info);
        }

        public Combat.CombatManager.InitializeInfo UnstageCombatInfo()
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
                Terminal.IO.LogWarning($"[GameInfo:InstantiatePlayer] ID '{id}' was invalid");
                return null;
            }
            CharacterStats player = this.playerTemplates.Get(id, this.playersFileManager);
            return player.Setup();
        }

        private SaveData LoadSaveData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Terminal.IO.LogWarning($"[GameInfo:InstantiateEnemy] ID '{id}' was invalid");
                return null;
            }

            string content = this.saveFilesManager.LoadFile(id);
            if (string.IsNullOrEmpty(content))
            {
                Terminal.IO.LogWarning($"[GameInfo:LoadSaveData] The contents of the save file at id '{id}' were empty");
                return null;
            }

            SaveData saveData = BF2D.Utilities.TextFile.DeserializeString<SaveData>(content);
            if (saveData is null)
            {
                Terminal.IO.LogError($"[GameInfo:LoadSaveData] The JSON at id '{id}' was invalid");
                return null;
            }

            foreach (CharacterStats character in saveData.ActivePlayers)
                character.Setup();

            return saveData;
        }
        #endregion
    }

}