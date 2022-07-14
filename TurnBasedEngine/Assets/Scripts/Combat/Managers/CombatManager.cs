using UnityEngine;
using System.Collections.Generic;
using BF2D.Game;
using DataStructures.PriorityQueue;
using System;
using BF2D.UI;
using BF2D.Game.Actions;
using BF2D.Enums;

namespace BF2D.Combat
{
    public class CombatManager : SceneManager
    {
        public class InitializeInfo
        {
            public List<PlayerStats> players = new();
            public List<EnemyStats> enemies = new();
        }

        [SerializeField] private UIOptionsControl mainMenu = null;
        [SerializeField] private ComboManager comboManager = null;
        [SerializeField] private StatsPreviewController statsPreviewController = null;
        [SerializeField] private CharacterTargeter targetManager = null;
        [SerializeField] private DialogTextboxControl textboxControl = null;
        [Header("Prefabs")]
        [SerializeField] private List<PlayerCombat> playerCombatPrefabs = new();
        [SerializeField] private List<EnemyCombat> enemyCombatPrefabs = new();

        private readonly Dictionary<string, PlayerCombat> playerCombatDict = new();
        private readonly Dictionary<string, EnemyCombat> enemyCombatDict = new();

        public List<CharacterStats> Characters
        {
            get
            {
                List<CharacterStats> statsList = new();

                foreach (PlayerCombat playerCombat in this.players)
                {
                    statsList.Add(playerCombat.Stats);
                }

                foreach (EnemyCombat enemyCombat in this.enemies)
                {
                    statsList.Add(enemyCombat.Stats);
                }

                return statsList;
            }
        }
        public List<PlayerStats> Players 
        { 
            get
            {
                List<PlayerStats> statsList = new();

                foreach (PlayerCombat playerCombat in this.players)
                {
                    statsList.Add(playerCombat.Stats);
                }

                return statsList;
            } 
        }
        private List<PlayerCombat> players = new();

        public List<EnemyStats> Enemies 
        { 
            get
            {
                List<EnemyStats> statsList = new();

                foreach (EnemyCombat enemyCombat in this.enemies)
                {
                    statsList.Add(enemyCombat.Stats);
                }

                return statsList;
            } 
        }
        private List<EnemyCombat> enemies = new();

        public PlayerStats CurrentPlayer 
        { 
            get
            {
                if (this.currentCharacter.Type != CharacterType.Player)
                    return null;

                try
                {
                    PlayerCombat playerCombat = (PlayerCombat)this.currentCharacter;
                    return playerCombat.Stats;

                }
                catch (Exception x)
                {
                    Debug.LogError(x.ToString());
                    return null;
                }
            }
        }

        public EnemyStats CurrentEnemy
        {
            get
            {
                if (this.currentCharacter.Type != CharacterType.Enemy)
                    return null;
                try
                {
                    EnemyCombat enemyCombat = (EnemyCombat)this.currentCharacter;
                    return enemyCombat.Stats;

                }
                catch (Exception x)
                {
                    Debug.LogError(x.ToString());
                    return null;
                }
            }
        }

        public CharacterStats CurrentCharacter
        {
            get
            {
                switch(CurrentCharacterType)
                {
                    case CharacterType.Player:
                        return CurrentPlayer;
                    case CharacterType.Enemy:
                        return CurrentEnemy;
                    default:
                        Debug.LogError($"[CombatManager] Tried to get the current character but the current character type '{CurrentCharacterType}' was invalid");
                        return null;
                }
            }
        }

        public CharacterType CurrentCharacterType { get { return this.currentCharacter.Type; } }
        private CharacterCombat currentCharacter = null;

        private readonly PriorityQueue<CharacterCombat, uint> characterQueue = new(0);

        private Action listener = null;

        public static CombatManager Instance { get { return CombatManager.instance; } }
        private static CombatManager instance = null;

        private void Awake()
        {
            SingletonSetup();
            this.listener = CombatInfoListen;
        }

        private void Update()
        {
            this.listener?.Invoke();
        }


        #region Public Utilities
        public void ExecuteCombatAction(ItemInfo itemInfo)
        {
            Item item = itemInfo.Get();

            SetupGameActionTargets(item.OnUse, () => this.currentCharacter.ExecuteCombatAction(new CharacterCombat.ItemStateData
            {
                item = item,
                itemInfo = itemInfo,
                gameAction = item.OnUse,
                textboxControl = this.textboxControl,
                dialog = new List<string> { "Stuff happened." }
            })); 
        }
        #endregion

        #region Listeners
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameInfo.Instance.CombatInfo;
            if (combatInfo is null)
                return;

            Initialize(combatInfo.players, combatInfo.enemies);
            this.listener = null;
        }
        #endregion

        #region Private Methods
        private void Initialize(List<PlayerStats> players, List<EnemyStats> enemies)
        {
            LoadCharacterPrefabs();

            foreach(PlayerStats playerStats in players)
            {
                this.players.Add(InstantiatePlayerCombat(playerStats));
            }

            foreach (EnemyStats enemyStats in enemies)
            {
                this.enemies.Add(InstantiateEnemyCombat(enemyStats));
            }

            FillCharacterQueue();

            UIControlsManager.Instance.TakeControl(this.textboxControl);
            this.textboxControl.Textbox.Message("Enemies approach.", () => BeginCombat());
        }

        private void LoadCharacterPrefabs()
        {
            foreach (PlayerCombat combatPrefab in this.playerCombatPrefabs)
            {
                this.playerCombatDict.Add(combatPrefab.name, combatPrefab);
            }

            foreach (EnemyCombat combatPrefab in this.enemyCombatPrefabs)
            {
                this.enemyCombatDict.Add(combatPrefab.name, combatPrefab);
            }
        }

        private PlayerCombat InstantiatePlayerCombat(PlayerStats playerStats)
        {
            if (playerStats is null)
            {
                Debug.LogWarning("[CombatManager] Tried to instantiate a PlayerCombat but the stats given were null");
                return null;
            }

            if (!this.playerCombatDict.ContainsKey(playerStats.ID))
            {
                Debug.LogError($"[CombatManager] Tried to instantiate the PlayerCombat at ID {playerStats.ID} but the ID could not be found");
            }

            PlayerCombat playerCombat = Instantiate(this.playerCombatDict[playerStats.ID]);
            playerCombat.Stats = playerStats;
            return playerCombat;
        }

        private EnemyCombat InstantiateEnemyCombat(EnemyStats enemyStats)
        {
            if (enemyStats is null)
            {
                Debug.LogWarning("[CombatManager] Tried to instantiate a PlayerCombat but the stats given were null");
                return null;
            }

            if (!this.enemyCombatDict.ContainsKey(enemyStats.ID))
            {
                Debug.LogError($"[CombatManager] Tried to instantiate the PlayerCombat at ID {enemyStats.ID} but the ID could not be found");
            }

            EnemyCombat enemyCombat = Instantiate(this.enemyCombatDict[enemyStats.ID]);
            enemyCombat.Stats = enemyStats;
            return enemyCombat;
        }

        private void BeginCombat()
        {
            this.currentCharacter = PopCharacterQueue();
            UIControlsManager.Instance.TakeControl(this.mainMenu);
        }

        private void FillCharacterQueue()
        {
            foreach(PlayerCombat combatControl in this.players)
            {
                this.characterQueue.Insert(combatControl, combatControl.Stats.Speed);
            }

            foreach (EnemyCombat combatControl in this.enemies)
            {
                this.characterQueue.Insert(combatControl, combatControl.Stats.Speed);
            }
        }

        private CharacterCombat PopCharacterQueue()
        {
            if (this.characterQueue.Top() is null)
                return null;
            return this.characterQueue.Pop();
        }

        private void SetupGameActionTargets(GameAction gameAction, Action callback)
        {
            foreach (CharacterStatsAction statsAction in gameAction.StatsActions)
            {
                this.targetManager.Enqueue(statsAction);
            }

            this.targetManager.Run(callback);
        }

        private void SingletonSetup()
        {
            //Setup of Monobehaviour Singleton
            if (CombatManager.instance is not null)
            {
                if (CombatManager.instance != this)
                {
                    Destroy(CombatManager.instance.gameObject);
                }
            }

            CombatManager.instance = this;
        }

        #endregion
    }
}
