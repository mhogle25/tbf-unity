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
    public class CombatManager : MonoBehaviour
    {
        public class InitializeInfo
        {
            public List<PlayerStats> players = new();
            public List<EnemyStats> enemies = new();
        }

        [SerializeField] private UIOptionsControl mainMenu = null;
        [SerializeField] private DialogTextboxControl textboxControl = null;
        [SerializeField] private CombatGrid combatGrid = null;
        [SerializeField] private CharacterTargeter characterTargeter = null;
        [Header("Prefabs")]
        [SerializeField] private List<PlayerCombat> playerCombatPrefabs = new();
        [SerializeField] private List<EnemyCombat> enemyCombatPrefabs = new();

        private readonly Dictionary<string, PlayerCombat> playerCombatDict = new();
        private readonly Dictionary<string, EnemyCombat> enemyCombatDict = new();

        public List<CharacterCombat> Characters
        {
            get
            {
                List<CharacterCombat> characterCombats = new();

                foreach (PlayerCombat playerCombat in this.players)
                {
                    characterCombats.Add(playerCombat);
                }

                foreach (EnemyCombat enemyCombat in this.enemies)
                {
                    characterCombats.Add(enemyCombat);
                }

                return characterCombats;
            }
        }
        public List<PlayerCombat> Players { get { return this.players; } }
        private List<PlayerCombat> players = new();

        public List<EnemyCombat> Enemies { get { return this.enemies; } }
        private List<EnemyCombat> enemies = new();

        public PlayerCombat CurrentPlayer 
        { 
            get
            {
                if (this.currentCharacter.Type != CharacterType.Player)
                    return null;

                try
                {
                    PlayerCombat playerCombat = (PlayerCombat)this.currentCharacter;
                    return playerCombat;

                }
                catch (Exception x)
                {
                    Debug.LogError(x.ToString());
                    return null;
                }
            }
        }

        public EnemyCombat CurrentEnemy
        {
            get
            {
                if (this.currentCharacter.Type != CharacterType.Enemy)
                    return null;
                try
                {
                    EnemyCombat enemyCombat = (EnemyCombat)this.currentCharacter;
                    return enemyCombat;

                }
                catch (Exception x)
                {
                    Debug.LogError(x.ToString());
                    return null;
                }
            }
        }

        public CharacterCombat CurrentCharacter { get { return this.currentCharacter; } }

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
        public void ExecuteItem(ItemInfo itemInfo)
        {
            Item item = itemInfo.Get();
            foreach (CharacterStatsAction statsAction in item.OnUse.StatsActions)
            {
                this.characterTargeter.Enqueue(statsAction);
            }
            this.characterTargeter.Run(() =>
            {

            });
        }
        #endregion

        #region Listeners
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameInfo.Instance.UnstageCombatInfo();
            if (combatInfo is null)
                return;

            foreach (EnemyStats stats in combatInfo.enemies)
            {
                Debug.Log(stats.Name);
            }

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

        private void BeginCombat()
        {
            this.currentCharacter = PopCharacterQueue();
            UIControlsManager.Instance.TakeControl(this.mainMenu);
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
            playerCombat.TextboxControl = this.textboxControl;
            this.combatGrid.UpdatePlayerPosition(playerCombat, playerCombat.Stats.GridPosition);
            return playerCombat;
        }

        private EnemyCombat InstantiateEnemyCombat(EnemyStats enemyStats)
        {
            if (enemyStats is null)
            {
                Debug.LogWarning("[CombatManager] Tried to instantiate a EnemyCombat but the stats given were null");
                return null;
            }

            if (!this.enemyCombatDict.ContainsKey(enemyStats.ID))
            {
                Debug.LogError($"[CombatManager] Tried to instantiate the EnemyCombat at ID {enemyStats.ID} but the ID could not be found");
            }

            EnemyCombat enemyCombat = Instantiate(this.enemyCombatDict[enemyStats.ID]);
            enemyCombat.Stats = enemyStats;
            enemyCombat.TextboxControl = this.textboxControl;
            this.combatGrid.UpdateEnemyPosition(enemyCombat, enemyCombat.Stats.GridPosition);
            return enemyCombat;
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
