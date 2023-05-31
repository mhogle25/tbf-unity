using System.Collections.Generic;
using BF2D.Game.Actions;
using UnityEngine;
using BF2D.Utilities;
using BF2D.Enums;
using System;
using BF2D.UI;

namespace BF2D.Game
{
    public class GameCtx : MonoBehaviour
    {
        //Singleton Reference
        public static GameCtx Instance => GameCtx.instance;
        private static GameCtx instance;

        public DialogTextboxControl SystemTextbox => this.systemTextbox;

        [Header("Data File Managers")]
        [SerializeField] private ExternalFileManager saveFilesManager = null;
        [SerializeField] private ExternalFileManager keyboardControlsConfigFilesManager = null;
        [SerializeField] private ExternalFileManager gamepadControlsConfigFilesManager = null;
        [SerializeField] private FileManager playersFileManager = null;
        [SerializeField] private FileManager enemiesFileManager = null;
        [SerializeField] private ExternalFileManager itemsFileManager = null;
        [SerializeField] private ExternalFileManager equipmentsFileManager = null;
        [SerializeField] private FileManager statusEffectsFileManager = null;
        [SerializeField] private FileManager gemsFileManager = null;
        [SerializeField] private FileManager runesFileManager = null;
        [SerializeField] private FileManager jobsFileManager = null;

        public bool SaveActive { get => this.currentSave is not null; }

        public CharacterStats PartyLeader => this.currentSave?.Party.PartyLeader;
        public IEnumerable<CharacterStats> ActivePlayers => this.currentSave?.Party.ActiveCharacters;

        public IItemHolder PartyItems => this.currentSave?.Party.Items;
        public IEquipmentHolder PartyEquipments => this.currentSave?.Party.Equipments;
        public ICharacterStatsActionHolder PartyGems => this.currentSave?.Party.Gems;

        public int Currency { get => this.currentSave.Party.Currency; set => this.currentSave.Party.Currency = value; }
        public int Ether { get => this.currentSave.Party.Ether; set => this.currentSave.Party.Ether = value; }

        private SaveData currentSave = null;

        //String Caches (instantiate on get, modifiable discardable data classes)
        private readonly JsonStringCache<CharacterStats> playerTemplates = new(5);
        private readonly JsonStringCache<CharacterStats> enemyTemplates = new(5);
        private readonly JsonStringCache<Item> itemTemplates = new(20);

        //Moddeable entity cloning cache
        private readonly JsonStringCache<Equipment> equipmentTemplates = new(10);

        //Object caches (no instantiation on get, single instance data classes)
        private readonly JsonEntityCache<Equipment> equipments = new(10);
        private readonly JsonEntityCache<StatusEffect> statusEffects = new(10);
        private readonly JsonEntityCache<CharacterStatsAction> gems = new(10);
        private readonly JsonEntityCache<EquipMod> runes = new(10);
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
            if (GameCtx.instance && GameCtx.instance != this)
                Destroy(GameCtx.instance.gameObject);

            GameCtx.instance = this;
        }

        #region Cache Management
        public void ClearCaches()
        {
            foreach (ICache cache in this.externalCaches)
                cache.Clear();

            this.playerTemplates.Clear();
            this.enemyTemplates.Clear();
            this.itemTemplates.Clear();

            this.equipmentTemplates.Clear();

            this.statusEffects.Clear();
            this.equipments.Clear();
            this.gems.Clear();
            this.runes.Clear();
            this.jobs.Clear();

            Terminal.IO.Log("Caches cleared");
        }

        public void RegisterCache(ICache cache)
        {
            this.externalCaches.Add(cache);
        }

        public void RemoveExternalCache(ICache cache)
        {
            this.externalCaches.Remove(cache);
        }
        #endregion

        #region Save Management
        public void NewGame(string saveID, string playerPrefabID, string playerName)
        {
            SaveData newGame = new SaveData().Setup(saveID);
            this.currentSave = newGame;
            NewPlayer(playerPrefabID, playerName);
        }

        public void SaveGame()
        {
            if (this.currentSave is null)
            {
                Debug.LogWarning("[GameInfo:SaveGame] Save failed, there was no game loaded.");
                return;
            }

            SaveGameAs(this.currentSave.ID);
        }

        public void SaveGameAs(string id)
        {
            if (this.currentSave is null)
            {
                Debug.LogWarning("[GameInfo:SaveGameAs] Save failed, there was no game loaded.");
                return;
            }

            this.currentSave.Setup(id);
            string newJSON = JSON.SerializeObject(this.currentSave);
            try
            {
                this.saveFilesManager.WriteToFile(id, newJSON);
            }
            catch (Exception x)
            {
                Debug.LogError(x.Message);
                return;
            }
            Terminal.IO.Log($"Saved at ID '{id}'");
        }

        public bool LoadGame()
        {
            if (this.currentSave is null)
            {
                Debug.LogWarning("[GameInfo:LoadGame] Load failed, there was no game loaded.");
                return false;
            }

            return LoadGame(this.currentSave.ID);
        }

        public bool LoadGame(string id)
        {
            this.currentSave = LoadSaveData(id);

            if (this.currentSave is null)
                return false;

            return true;
        }

        private SaveData LoadSaveData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:InstantiateEnemy] ID '{id}' was invalid");
                return null;
            }

            string content = this.saveFilesManager.LoadFile(id);
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogWarning($"[GameInfo:LoadSaveData] The contents of the save file at id '{id}' were empty");
                return null;
            }

            SaveData saveData = JSON.DeserializeString<SaveData>(content);
            if (saveData is null)
            {
                Debug.LogError($"[GameInfo:LoadSaveData] The JSON at id '{id}' was invalid");
                return null;
            }

            return saveData.Setup(id);
        }
        #endregion

        #region Controls Config Management
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

            try
            {
                switch (controllerType)
                {
                    case InputController.Keyboard:
                        InputManager.Instance.KeyboardID = id;
                        this.keyboardControlsConfigFilesManager.WriteToFile(id, newJSON);
                        break;
                    case InputController.Gamepad:
                        InputManager.Instance.GamepadID = id;
                        this.gamepadControlsConfigFilesManager.WriteToFile(id, newJSON);
                        break;
                    default:
                        Debug.LogError("[GameInfo:SaveControlsConfigAs] InputController enum value was invalid");
                        break;
                }
            }
            catch (Exception x)
            {
                Debug.LogError(x.Message);
                return;
            }

            Terminal.IO.Log($"{controllerType} config saved at ID '{id}'");
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

            InputManager.Instance.DeserializeConfig(controllerType, newJSON);

            Terminal.IO.Log($"{controllerType} config with id '{id}' was loaded");
        }
        #endregion

        #region Character Management
        public CharacterStats GetActivePlayer(string id)
        {
            return this.currentSave?.Party.GetCharacter(id);
        }

        public void NewPlayer(string id, string newName)
        {
            CharacterStats newPlayer = InstantiatePlayer(id);
            if (newPlayer is null)
                return;

            newPlayer.Name = newName;
            this.currentSave.Party.AddCharacter(newPlayer);
        }

        public CharacterStats InstantiateEnemy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:InstantiateEnemy] ID '{id}' was invalid");
                return null;
            }
            CharacterStats enemy = this.enemyTemplates.Get(id, this.enemiesFileManager);
            return enemy;
        }

        private CharacterStats InstantiatePlayer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:InstantiatePlayer] ID '{id}' was invalid");
                return null;
            }

            CharacterStats player = this.playerTemplates.Get(id, this.playersFileManager);
            return player;
        }
        #endregion

        #region Combat Management
        public void StageCombatInfo(Combat.CombatManager.InitializeInfo info)
        {
            this.queuedCombats.Enqueue(info);
        }

        public Combat.CombatManager.InitializeInfo UnstageCombatInfo()
        {
            if (this.queuedCombats.Count < 1)
                return null;

            return this.queuedCombats.Dequeue();
        }
        #endregion

        #region Static Asset Getters
        public Sprite GetIcon(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:GetIcon] ID '{id}' was invalid");
                return null;
            }

            return this.iconCollection[id];
        }

        public AudioClip GetSoundEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:GetSoundEffect] ID '{id}' was invalid");
                return null;
            }

            return this.soundEffectCollection[id];
        }
        #endregion

        #region Entity Getters

        public Equipment GetEquipment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:GetEquipment] ID '{id}' was invalid");
                return null;
            }

            return this.equipments.Get(id, this.equipmentsFileManager);
        }

        public StatusEffect GetStatusEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:GetStatusEffect] ID '{id}' was invalid");
                return null;
            }

            return this.statusEffects.Get(id, this.statusEffectsFileManager);
        }

        public CharacterStatsAction GetGem(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:GetGem] ID '{id}' was invalid");
                return null;
            }

            return this.gems.Get(id, this.gemsFileManager);
        }

        public EquipMod GetRune(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:GetRune] ID '{id}' was invalid");
                return null;
            }

            return this.runes.Get(id, this.runesFileManager);
        }

        public Job GetJob(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:GetJob] ID '{id}' was invalid");
            }

            return this.jobs.Get(id, this.jobsFileManager);
        }
        #endregion

        #region Entity Instantiators
        public Item InstantiateItem(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:InstantiateItem] ID '{id}' was invalid");
                return null;
            }

            return this.itemTemplates.Get(id, this.itemsFileManager);
        }

        public Equipment InstantiateEquipment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"[GameInfo:InstantiateItem] ID '{id}' was invalid");
                return null;
            }

            return this.equipmentTemplates.Get(id, this.itemsFileManager);
        }
        #endregion

        #region Entity Write/Overwrite
        /// <summary>
        /// Wraps an item in a callback object that will write/overwrite that entity at its ID
        /// </summary>
        /// <param name="item">Item to write</param>
        /// <returns>Item file writer</returns>
        public FileWriter WriteItem(Item item, Action callback) => new FileWriter(
            this.itemsFileManager,
            item.ID,
            Utilities.JSON.SerializeObject(item),
            callback
        );

        /// <summary>
        /// Wraps an equipment in a callback object that will write/overwrite that entity at its ID
        /// </summary>
        /// <param name="equipment">Equipment to write</param>
        /// <returns>Equipment file writer</returns>
        public FileWriter WriteEquipment(Equipment equipment, Action callback) => new FileWriter(
            this.equipmentsFileManager,
            equipment.ID,
            Utilities.JSON.SerializeObject(equipment),
            callback
        );
        #endregion

        #region Custom Entity Delete
        public void DeleteItemIfCustom(string id)
        {
            if (!this.itemsFileManager.FileExists(id))
            {
                Debug.LogError($"[GameInfo:DeleteItem] The file at ID {id} does not exist.");
                return;
            }

            if (this.itemsFileManager.FileExists(id, GameDirectory.Streaming))
                return;

            this.itemsFileManager.DeleteFile(id);
        }

        public void DeleteEquipmentIfCustom(string id)
        {
            if (!this.equipmentsFileManager.FileExists(id))
            {
                Debug.LogError($"[GameInfo:DeleteItem] The file at ID {id} does not exist.");
                return;
            }

            if (this.equipmentsFileManager.FileExists(id, GameDirectory.Streaming))
                return;

            this.equipmentsFileManager.DeleteFile(id);
        }
        #endregion
    }

}