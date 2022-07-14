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

        [Header("Data File Paths")]
        [SerializeField] private string savesPath = "Saves";

        [SerializeField] private string playersPath = "Players";
        [SerializeField] private string enemiesPath = "Enemies";
        [SerializeField] private string itemsPath = "Items";
        [SerializeField] private string equipmentsPath = "Equipments";
        [SerializeField] private string statusEffectsPath = "StatusEffects";

        public List<PlayerStats> Players { get { return this.currentSave.Players; } }
        private SaveData currentSave = null;

        private readonly JsonCache<Item> items = new JsonCache<Item>();
        private readonly JsonCache<Equipment> equipments = new JsonCache<Equipment>();
        private readonly JsonCache<StatusEffect> statusEffects = new JsonCache<StatusEffect>();

        //AssetCollections
        [Header("Asset Collections")]
        [SerializeField] private SpriteCollection iconCollection = null;
        [SerializeField] private AudioClipCollection soundEffectCollection = null;

        public CombatManager.InitializeInfo CombatInfo 
        { 
            get 
            {
                if (this.combatInfos.Count < 1)
                    return null;
                return this.combatInfos.Dequeue(); 
            } 
        }
        private Queue<CombatManager.InitializeInfo> combatInfos = new Queue<CombatManager.InitializeInfo>();

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
            this.currentSave = LoadSaveData("save1");
            List<EnemyStats> enemies = new List<EnemyStats>();
            enemies.Add(LoadEnemy("lessergoblin"));
            this.combatInfos.Enqueue(new CombatManager.InitializeInfo
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
            Item item = this.items[key];
            return item;
        }

        public Equipment GetEquipment(string key)
        {
            this.equipments.Datapath = Path.Combine(Application.streamingAssetsPath, this.equipmentsPath);
            Equipment equipment = this.equipments[key];
            return equipment;
        }

        public StatusEffect GetStatusEffect(string key)
        {
            this.statusEffects.Datapath = Path.Combine(Application.streamingAssetsPath, this.statusEffectsPath);
            StatusEffect statusEffect = this.statusEffects[key];
            return statusEffect;
        }

        public EnemyStats LoadEnemy(string key)
        {
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.streamingAssetsPath, this.enemiesPath, key + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<EnemyStats>(content);
        }

        public void AddPlayer(string playerKey, string newName)
        {
            PlayerStats newPlayer = LoadPlayer(playerKey);
            newPlayer.Name = newName;
            this.currentSave.AddPlayer(newPlayer);
        }

        private PlayerStats LoadPlayer(string key)
        {
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.streamingAssetsPath, this.playersPath, key + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<PlayerStats>(content);
        }

        private SaveData LoadSaveData(string saveKey)
        {
            string content = BF2D.Utilities.TextFile.LoadFile(Path.Combine(Application.persistentDataPath, this.savesPath, saveKey + ".json"));
            return BF2D.Utilities.TextFile.DeserializeString<SaveData>(content);
        }
    }

}