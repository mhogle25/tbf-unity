using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using BF2D.Combat;

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
         
        public List<CharacterStats> Players { get { return this.currentSave.Players; } }
        private SaveData currentSave = null;

        private readonly JsonStringCache<Item> items = new();
        private readonly JsonObjectCache<Equipment> equipments = new();
        private readonly JsonObjectCache<StatusEffect> statusEffects = new();

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
                InstantiateEnemy("lessergoblin")
            };

            this.queuedCombats.Enqueue(new CombatManager.InitializeInfo
            {
                players = this.Players,
                enemies = enemies
            });
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
            Item item = this.items.Get(id);
            return item;
        }

        public Equipment GetEquipment(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetEquipment] String was empty");
                return null;
            }
            this.equipments.Datapath = Path.Combine(Application.streamingAssetsPath, this.equipmentsPath);
            Equipment equipment = this.equipments.Get(id);
            return equipment;
        }

        public StatusEffect GetStatusEffect(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:GetEquipment] String was empty");
                return null;
            }

            this.statusEffects.Datapath = Path.Combine(Application.streamingAssetsPath, this.statusEffectsPath);
            StatusEffect statusEffect = this.statusEffects.Get(id);
            return statusEffect;
        }

        public CharacterStats InstantiateEnemy(string id)
        {
            if (id == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] String was empty");
                return null;
            }
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.streamingAssetsPath, this.enemiesPath, id + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<CharacterStats>(content).Setup();
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
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.streamingAssetsPath, this.playersPath, id + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<CharacterStats>(content).Setup();
        }

        private SaveData LoadSaveData(string saveID)
        {
            if (saveID == string.Empty)
            {
                Debug.LogWarning("[GameInfo:InstantiateEnemy] String was empty");
                return null;
            }
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.persistentDataPath, this.savesPath, saveID + ".json"));
            SaveData saveData = BF2D.Utilities.TextFile.DeserializeString<SaveData>(content);

            foreach (CharacterStats character in saveData.Players)
            {
                character.Setup();
            }

            return saveData;
        }
    }

}