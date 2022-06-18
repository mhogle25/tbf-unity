using UnityEngine;
using System.Collections.Generic;
using BF2D.Game;
using DataStructures.PriorityQueue;
using BF2D.Actions;
using System;
using BF2D.UI;

namespace BF2D.Combat
{
    public class CombatManager : SceneManager
    {
        public class InitializeInfo
        {
            public List<PlayerStats> players = new List<PlayerStats>();
            public List<EnemyStats> enemies = new List<EnemyStats>();
        }

        [SerializeField] private UIOptionsControl mainMenu = null;
        [SerializeField] private ComboManager comboManager = null;
        [SerializeField] private StatsPreviewController statsPreviewController = null;
        [SerializeField] private CharacterTargetManager targetManager = null;
        [SerializeField] private DialogTextboxControl textboxControl = null;

        public List<CharacterStats> Characters
        {
            get
            {
                List<CharacterStats> statsList = new List<CharacterStats>();

                foreach (PlayerStats playerCombat in this.players)
                {
                    statsList.Add(playerCombat);
                }

                foreach (EnemyStats enemyCombat in this.enemies)
                {
                    statsList.Add(enemyCombat);
                }

                return statsList;
            }
        }
        public List<PlayerStats> Players { get { return this.players; } }
        private List<PlayerStats> players = new List<PlayerStats>();
        public List<EnemyStats> Enemies { get { return this.enemies; } }
        private List<EnemyStats> enemies = new List<EnemyStats>();

        public CharacterStats CurrentPlayer 
        { 
            get
            {
                if (this.currentCharacter.Type != Enums.CharacterType.Player)
                    return null;

                try
                {
                    PlayerStats playerStats = (PlayerStats)this.currentCharacter;
                    return playerStats;
                } catch (Exception x)
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
                if (this.currentCharacter.Type != Enums.CharacterType.Enemy)
                    return null;

                try
                {
                    EnemyStats enemyStats = (EnemyStats)this.currentCharacter;
                    return enemyStats;
                }
                catch (Exception x)
                {
                    Debug.LogError(x.ToString());
                    return null;
                }
            }
        }
        public CharacterStats CurrentCharacter { get { return this.currentCharacter; } }
        private CharacterStats currentCharacter = null;

        private PriorityQueue<CharacterStats, uint> characterQueue = new PriorityQueue<CharacterStats, uint>(0);

        private CombatState state = null;
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

        private void SingletonSetup()
        {
            //Setup of Monobehaviour Singleton
            if (CombatManager.instance != null)
            {
                if (CombatManager.instance != this)
                {
                    Destroy(CombatManager.instance.gameObject);
                }
            }

            CombatManager.instance = this;
        }

        #region 
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameInfo.Instance.CombatInfo;
            if (combatInfo is null)
                return;

            Initialize(combatInfo.players, combatInfo.enemies);
            this.listener = null;
        }
        #endregion

        private void Initialize(List<PlayerStats> players, List<EnemyStats> enemies)
        {
            this.players = players;
            this.enemies = enemies;

            FillCharacterQueue();

            UIControlsManager.Instance.TakeControl(this.textboxControl);
            this.textboxControl.Textbox.Message("Enemies approach.", () => BeginCombat());
        }

        private void BeginCombat()
        {
            this.currentCharacter = PopCharacterQueue();
            UIControlsManager.Instance.TakeControl(this.mainMenu);
        }

        private void FillCharacterQueue()
        {
            foreach(PlayerStats stats in this.players)
            {
                this.characterQueue.Insert(stats, stats.Speed);
            }

            foreach (EnemyStats stats in this.enemies)
            {
                this.characterQueue.Insert(stats, stats.Speed);
            }
        }

        private CharacterStats PopCharacterQueue()
        {
            if (this.characterQueue.Top() is null)
                return null;
            return this.characterQueue.Pop();
        }

        public void ExecuteItem(ItemInfo itemInfo)
        {
            Item item = itemInfo.Get();

            SetupGameAction(item.OnUse, () => this.state.Begin());

            this.state = new StateHandleItem
            {
                TextboxControl = this.textboxControl,
                CurrentItem = item,
                CurrentItemInfo = itemInfo,
                Finalize = () =>
                {

                }
            };
        }

        /*
        public void ExecuteGameAction(GameAction gameAction)
        {
            SetupGameAction(gameAction, () => this.state.ExecuteGameAction(gameAction));
        }
        */

        private void SetupGameAction(GameAction gameAction, Action callback)
        {
            foreach (CharacterStatsAction statsAction in gameAction.StatsActions)
            {
                this.targetManager.Enqueue(statsAction);
            }

            this.targetManager.Execute(callback);
        }
    }
}
