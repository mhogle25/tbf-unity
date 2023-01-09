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

        private readonly JsonCache<Item> items = new();
        private readonly JsonCache<Equipment> equipments = new();
        private readonly JsonCache<StatusEffect> statusEffects = new();

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
                LoadEnemy("lessergoblin")
            };

            this.queuedCombats.Enqueue(new CombatManager.InitializeInfo
            {
                players = this.Players,
                enemies = enemies
            });
        }

        public Sprite GetIcon(string key)
        {
            return this.iconCollection[key];
        }

        public AudioClip GetSoundEffect(string key)
        {
            return this.soundEffectCollection[key];
        }

        public Item GetItem(string key)
        {
            this.items.Datapath = Path.Combine(Application.streamingAssetsPath, this.itemsPath);
            Item item = this.items.Get(key);
            return item;
        }

        public Equipment GetEquipment(string key)
        {
            this.equipments.Datapath = Path.Combine(Application.streamingAssetsPath, this.equipmentsPath);
            Equipment equipment = this.equipments.Get(key);
            return equipment;
        }

        public StatusEffect GetStatusEffect(string key)
        {
            this.statusEffects.Datapath = Path.Combine(Application.streamingAssetsPath, this.statusEffectsPath);
            StatusEffect statusEffect = this.statusEffects.Get(key);
            return statusEffect;
        }

        public CharacterStats LoadEnemy(string key)
        {
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.streamingAssetsPath, this.enemiesPath, key + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<CharacterStats>(content);
        }

        public void AddPlayer(string playerKey, string newName)
        {
            CharacterStats newPlayer = LoadPlayer(playerKey);
            newPlayer.SetName(newName);
            this.currentSave.AddPlayer(newPlayer);
        }

        private CharacterStats LoadPlayer(string key)
        {
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.streamingAssetsPath, this.playersPath, key + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<CharacterStats>(content);
        }

        private SaveData LoadSaveData(string saveKey)
        {
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.persistentDataPath, this.savesPath, saveKey + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<SaveData>(content);
        }
    }

}