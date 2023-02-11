using UnityEngine;
using System.Collections.Generic;
using BF2D.Game;
using System;
using BF2D.UI;
using BF2D.Combat.Actions;
using BF2D.Game.Actions;
using System.Threading;
using System.Threading.Tasks;

namespace BF2D.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public class InitializeInfo
        {
            public List<CharacterStats> players = new();
            public List<CharacterStats> enemies = new();
            public string openingDialogKey = "di_opening_default";
        }

        [SerializeField] private OptionsGridControlInit mainMenu = null;

        [SerializeField] private DialogTextboxControl standaloneTextboxControl = null;
        [SerializeField] private DialogTextbox orphanedTextbox = null;

        [SerializeField] private CombatGrid combatGrid = null;
        [SerializeField] private CharacterTargeterControl characterTargeter = null;

        private Action listener = null;

        private static CombatManager instance = null;
        public static CombatManager Instance { get { return CombatManager.instance; } }

        public CharacterCombat CurrentCharacter { get { return this.combatGrid.CurrentCharacter; } }
        public IEnumerable<CharacterCombat> Players { get { return this.combatGrid.Players; } }
        public int PlayerCount { get { return this.combatGrid.PlayerCount; } }
        public IEnumerable<CharacterCombat> Enemies { get { return this.combatGrid.Enemies; } }
        public int EnemyCount { get { return this.combatGrid.EnemyCount; } }
        public IEnumerable<CharacterCombat> Characters { get { return this.combatGrid.Characters; } }
        public CharacterTargeterControl CharacterTargeter { get { return this.characterTargeter; } }
        public DialogTextbox OrphanedTextbox { get { return this.orphanedTextbox; } }

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
        public void SetupItemCombat(ItemInfo itemInfo)
        {
            this.CurrentCharacter.SetupCombatAction(new CombatAction
            {
                Item = new ItemCombatActionInfo
                {
                    Info = itemInfo
                }
            });
        }

        public void RunCombatEvents()
        {
            UIControlsManager.Instance.ResetControlChain(false);
            this.CurrentCharacter.RunCombatEvents();
        }

        public void PassTurn()
        {
            ResetOrphanedTextbox();

            this.combatGrid.PassTurn();
            BeginTurn();
        }

        public void EndCombat()
        {
            ResetOrphanedTextbox();
            this.combatGrid.PassTurn();
            string dialogKey = EnemiesAreDefeated() ? "di_victory" : PlayersAreDefeated() ? "di_defeat" : "di_draw";
            this.standaloneTextboxControl.Textbox.Dialog(dialogKey, 0, () =>
            {
                Terminal.IO.Log("Final Trigger");
                CancelCombat();
            });

            UIControlsManager.Instance.TakeControl(this.standaloneTextboxControl);
        }

        public void CancelCombat()
        {
            this.combatGrid.GridReset();
            UIControlsManager.Instance.ResetControlChain(false);
            ResetOrphanedTextbox();
            this.listener = CombatInfoListen;
        }

        public void ResetOrphanedTextbox()
        {
            this.OrphanedTextbox.UtilityFinalize();
            this.OrphanedTextbox.View.gameObject.SetActive(false);
        }

        public bool CombatIsOver()
        {
            return EnemiesAreDefeated() || PlayersAreDefeated();
        }

        public bool EnemiesAreDefeated()
        {
            bool enemiesDefeated = true;
            foreach (CharacterCombat enemy in this.Enemies)
            {
                enemiesDefeated = enemiesDefeated && enemy.Stats.Dead;
            }
            return enemiesDefeated;
        }

        public bool PlayersAreDefeated()
        {
            bool playersDefeated = true;
            foreach (CharacterCombat player in this.Players)
            {
                playersDefeated = playersDefeated && player.Stats.Dead;
            }
            return playersDefeated;
        }
        #endregion

        #region Listeners
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameInfo.Instance.UnstageCombatInfo();
            if (combatInfo is null)
                return;

            Initialize(combatInfo);
            this.listener = null;
        }
        #endregion

        #region Private Methods
        private void Initialize(InitializeInfo initInfo)
        {
            this.combatGrid.Setup(initInfo.players, initInfo.enemies);

            this.standaloneTextboxControl.Textbox.Dialog(initInfo.openingDialogKey, 0, BeginTurn);
            UIControlsManager.Instance.TakeControl(this.standaloneTextboxControl);

            Terminal.IO.LogQuiet($"Combat intialized with {CombatManager.Instance.PlayerCount} players and {CombatManager.Instance.EnemyCount} enemies.");
        }

        private void BeginTurn()
        {
            if (this.combatGrid.CharacterIsPlayer(this.CurrentCharacter))
            {
                UIControlsManager.Instance.TakeControl(this.mainMenu);
                return;
            }

            //Begin Enemy AI
            //
            //
            //
            //
            //
            //
            //GameInfo.Instance.SaveGame();
            UIControlsManager.Instance.TakeControl(this.mainMenu);
            //
            //
            //
            //
            //
            //
            //
        }

        private void SingletonSetup()
        {
            //Setup of Monobehaviour Singleton
            if (CombatManager.instance)
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
