using System;
using System.Collections.Generic;
using UnityEngine;
using BF2D.Game.Actions;
using BF2D.Utilities;
using BF2D.Enums;
using System.Linq;

namespace BF2D.Game
{
    class GameCtx : MonoBehaviourSingleton<GameCtx>
    {
        [Header("Data File Managers")]
        [SerializeField] private ExternalFileManager saveFilesManager = null;
        [SerializeField] private ExternalFileManager keyboardControlsConfigFilesManager = null;
        [SerializeField] private ExternalFileManager gamepadControlsConfigFilesManager = null;
        [SerializeField] private FileManager playersFileManager = null;
        [SerializeField] private FileManager enemiesFileManager = null;
        [SerializeField] private ExternalFileManager itemsFileManager = null;
        [SerializeField] private ExternalFileManager equipmentsFileManager = null;
        [SerializeField] private FileManager statusEffectsFileManager = null;
        [SerializeField] private ExternalFileManager gemsFileManager = null;
        [SerializeField] private ExternalFileManager runesFileManager = null;
        [SerializeField] private FileManager jobsFileManager = null;
        [SerializeField] private FileManager encounterFactoriesFileManager = null;

        //AssetCollections
        [Header("Asset Collections")]
        [SerializeField] private SpriteCollection iconCollection = null;
        [SerializeField] private AudioClipCollection soundEffectCollection = null;

        public bool SaveActive => this.currentSave is not null;

        public CharacterStats[] ActivePlayers => this.currentSave?.Party.ActiveCharacters.Select(character => character.Stats).ToArray();

        public IItemHolder Items => this.currentSave?.Party.Items;
        public IEquipmentHolder Equipment => this.currentSave?.Party.Equipment;
        public ICharacterActionHolder Gems => this.currentSave?.Party.Gems;
        public IEquipModHolder Runes => this.currentSave?.Party.Runes;

        public int Currency { get => this.currentSave?.Party?.Currency ?? 0; set => this.currentSave.Party.Currency = value; }
        public int Ether { get => this.currentSave?.Party?.Ether ?? 0; set => this.currentSave.Party.Ether = value; }

        public CharacterStats PartyLeader => this.currentSave?.Party.Leader.Stats;

        private SaveData currentSave = null;

        //String Caches (instantiate on get, mutable discardable data classes)
        private readonly JsonStringCache<CharacterStats> playerTemplates = new(5);
        private readonly JsonStringCache<CharacterStats> enemyTemplates = new(5);
        private readonly JsonStringCache<Item> itemTemplates = new(20);

        //Mutable entity cloning cache
        private readonly JsonStringCache<Equipment> equipmentTemplates = new(10);
        private readonly JsonStringCache<CharacterAction> gemTemplates = new(10);

        //Immutable object caches (no instantiation on get, single instance data classes)
        private readonly JsonEntityCache<Equipment> equipments = new(10);
        private readonly JsonEntityCache<StatusEffect> statusEffects = new(10);
        private readonly JsonEntityCache<CharacterAction> gems = new(10);
        private readonly JsonEntityCache<EquipMod> runes = new(10);
        private readonly JsonEntityCache<Job> jobs = new(10);
        private readonly JsonEntityCache<EncounterFactory> encounterFactories = new(10);

        private readonly List<ICache> externalCaches = new();

        private readonly Queue<Combat.CombatCtx.InitializeInfo> queuedCombats = new();

        #region Cache Management
        public void ClearCaches()
        {
            foreach (ICache cache in this.externalCaches)
                cache.Clear();

            this.playerTemplates.Clear();
            this.enemyTemplates.Clear();
            this.itemTemplates.Clear();

            this.equipmentTemplates.Clear();
            this.gemTemplates.Clear();

            this.statusEffects.Clear();
            this.equipments.Clear();
            this.gems.Clear();
            this.runes.Clear();
            this.jobs.Clear();
            this.encounterFactories.Clear();

            ShCtx.One.Log("Caches cleared");
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
                Debug.LogWarning("[GameContext:SaveGame] Save failed, there was no game loaded.");
                return;
            }

            SaveGameAs(this.currentSave.ID);
        }

        public void SaveGameAs(string id)
        {
            if (this.currentSave is null)
            {
                Debug.LogWarning("[GameContext:SaveGameAs] Save failed, there was no game loaded.");
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
            ShCtx.One.Log($"Saved at ID '{id}'");
        }

        public bool ReloadGame()
        {
            if (this.currentSave is null)
            {
                Debug.LogWarning("[GameContext:ReloadGame] Reload failed, there was no game loaded.");
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
                Debug.LogWarning($"[GameContext:InstantiateEnemy] ID '{id}' was invalid");
                return null;
            }

            string content = this.saveFilesManager.LoadFile(id);
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogWarning($"[GameContext:LoadSaveData] The contents of the save file at id '{id}' were empty");
                return null;
            }

            SaveData saveData = JSON.DeserializeJson<SaveData>(content);
            if (saveData is null)
            {
                Debug.LogError($"[GameContext:LoadSaveData] The JSON at id '{id}' was invalid");
                return null;
            }

            return saveData.Setup(id);
        }
        #endregion

        #region Controls Config Management
        public void NewControlsConfig(InputController controllerType)
        {
            InputCtx.One.ResetConfig(controllerType);
        }

        public void SaveControlsConfig(InputController controllerType)
        {
            string id = controllerType switch
            {
                InputController.Keyboard => InputCtx.One.KeyboardID,
                InputController.Gamepad => InputCtx.One.GamepadID,
                _ => throw new ArgumentException("[GameContext:SaveControlsConfig] InputController enum value was invalid")
            };
            SaveControlsConfigAs(controllerType, id);
        }

        public void SaveControlsConfigAs(InputController controllerType, string id)
        {
            string newJSON = InputCtx.One.SerializeConfig(controllerType);

            try
            {
                switch (controllerType)
                {
                    case InputController.Keyboard:
                        InputCtx.One.KeyboardID = id;
                        this.keyboardControlsConfigFilesManager.WriteToFile(id, newJSON);
                        break;
                    case InputController.Gamepad:
                        InputCtx.One.GamepadID = id;
                        this.gamepadControlsConfigFilesManager.WriteToFile(id, newJSON);
                        break;
                    default:
                        Debug.LogError("[GameContext:SaveControlsConfigAs] InputController enum value was invalid");
                        break;
                }
            }
            catch (Exception x)
            {
                Debug.LogError(x.Message);
                return;
            }

            ShCtx.One.Log($"{controllerType} config saved at ID '{id}'");
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
                Debug.LogError("[GameContext:LoadControlsConfig] Fetch failed");
                return;
            }

            InputCtx.One.DeserializeConfig(controllerType, newJSON);

            ShCtx.One.Log($"{controllerType} config with id '{id}' was loaded");
        }
        #endregion

        #region Character Management
        public ICharacterInfo NewPlayer(string id, string newName)
        {
            CharacterStats newPlayer = InstantiatePlayer(id);
            if (newPlayer is null)
                return null;

            newPlayer.Name = newName;
            return this.currentSave.Party.AddPlayer(newPlayer);
        }

        /// <summary>
        /// Makes a character reference from the current party that party's leader
        /// </summary>
        /// <param name="newLeader">The new leader</param>
        /// <returns>The old leader</returns>
        public ICharacterInfo MakePlayerLeader(ICharacterInfo newLeader) => this.currentSave?.Party.ChangeLeader(newLeader);

        public CharacterStats InstantiateEnemy(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            CharacterStats enemy = this.enemyTemplates.Get(id, this.enemiesFileManager);
            return enemy;
        }

        private CharacterStats InstantiatePlayer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            CharacterStats player = this.playerTemplates.Get(id, this.playersFileManager);
            return player;
        }
        #endregion

        #region Combat Management
        public void StageEncounter(Encounter encounter)
        {
            this.queuedCombats.Enqueue(new Combat.CombatCtx.InitializeInfo
            {
                players = this.currentSave.Party,
                enemies = encounter
            });
        }

        public Combat.CombatCtx.InitializeInfo UnstageEncounter()
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
                return null;

            return this.iconCollection[id];
        }

        public AudioClip GetSoundEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.soundEffectCollection[id];
        }
        #endregion

        #region Entity Getters
        public Equipment GetEquipment(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.equipments.Get(id, this.equipmentsFileManager);
        }

        public StatusEffect GetStatusEffect(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.statusEffects.Get(id, this.statusEffectsFileManager);
        }

        public CharacterAction GetGem(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.gems.Get(id, this.gemsFileManager);
        }

        public EquipMod GetRune(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.runes.Get(id, this.runesFileManager);
        }

        public Job GetJob(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.jobs.Get(id, this.jobsFileManager);
        }

        public EncounterFactory GetEncounterFactory(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.encounterFactories.Get(id, this.encounterFactoriesFileManager);
        }
        #endregion

        #region Entity Instantiators
        public Item InstantiateItem(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.itemTemplates.Get(id, this.itemsFileManager);
        }

        public Equipment InstantiateEquipment(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.equipmentTemplates.Get(id, this.equipmentsFileManager);
        }

        public CharacterAction InstantiateGem(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return this.gemTemplates.Get(id, this.gemsFileManager);
        }
        #endregion

        #region Entity Write/Overwrite
        /// <summary>
        /// Wraps an item in a callback object that will write/overwrite that entity at its ID
        /// </summary>
        /// <param name="item">Item to write</param>
        /// <returns>Item file writer</returns>
        public FileWriter WriteItem(Item item, Action callback) => new(
            this.itemsFileManager,
            item.ID,
            JSON.SerializeObject(item),
            callback
        );

        /// <summary>
        /// Wraps an equipment in a callback object that will write/overwrite that entity at its ID
        /// </summary>
        /// <param name="equipment">Equipment to write</param>
        /// <returns>Equipment file writer</returns>
        public FileWriter WriteEquipment(Equipment equipment, Action callback) => new(
            this.equipmentsFileManager,
            equipment.ID,
            JSON.SerializeObject(equipment),
            callback
        );

        /// <summary>
        /// Wraps a gem in a callback object that will write/overwrite that entity at its ID
        /// </summary>
        /// <param name="gem">Gem to write</param>
        /// <returns>Gem file writer</returns>
        public FileWriter WriteGem(CharacterAction gem, Action callback) => new(
            this.gemsFileManager,
            gem.ID,
            JSON.SerializeObject(gem),
            callback
        );
        #endregion

        #region Custom Entity Delete
        public void DeleteItemIfCustom(string id)
        {
            DeleteEntityIfCustom(id, this.itemsFileManager, "Item");
        }

        public void DeleteEquipmentIfCustom(string id)
        {
            DeleteEntityIfCustom(id, this.equipmentsFileManager, "Equipment");
        }

        public void DeleteGemIfCustom(string id)
        {
            DeleteEntityIfCustom(id, this.gemsFileManager, "Gem");
        }

        public void DeleteRuneIfCustom(string id)
        {
            DeleteEntityIfCustom(id, this.runesFileManager, "Rune");
        }

        private void DeleteEntityIfCustom(string id, ExternalFileManager fileManager, string label)
        {
            if (!fileManager.FileExists(id))
            {
                Debug.LogError($"[GameContext:Delete{label}IfCustom] The file at ID {id} does not exist.");
                return;
            }

            if (fileManager.FileExists(id, GameDirectory.Streaming))
                return;

            fileManager.DeleteFile(id);
            ClearCaches();
        }
        #endregion
    }
}