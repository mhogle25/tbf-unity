using UnityEngine;
using System.Collections.Generic;
using BF2D.Game;
using DataStructures.PriorityQueue;
using System;
using BF2D.UI;
using BF2D.Combat.Actions;
using BF2D.Enums;

namespace BF2D.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public class InitializeInfo
        {
            public List<CharacterStats> players = new();
            public List<CharacterStats> enemies = new();
        }

        [SerializeField] private OptionsGridControlInit mainMenu = null;
        [SerializeField] private DialogTextboxControl textboxControl = null;
        [SerializeField] private CombatGrid combatGrid = null;
        [SerializeField] private CharacterTargeterControl characterTargeter = null;

        private static CombatManager instance = null;

        private Action listener = null;

        public static CombatManager Instance { get { return CombatManager.instance; } }

        public CharacterCombat CurrentCharacter { get { return this.combatGrid.CurrentCharacter; } }
        public IEnumerable<CharacterCombat> Players { get { return this.combatGrid.Players; } }
        public IEnumerable<CharacterCombat> Enemies { get { return this.combatGrid.Enemies; } }
        public IEnumerable<CharacterCombat> Characters { get { return this.combatGrid.Characters; } }
        public CharacterTargeterControl CharacterTargeter { get { return this.characterTargeter; } }

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
            this.CurrentCharacter.SetupCombatAction(new CombatAction
            {
                CombatActionType = CombatActionType.Item,
                Item = new ItemCombatActionInfo
                {
                    Info = itemInfo
                }
            });
        }

        public void RunCombat()
        {
            //TODO
        }
        #endregion

        #region Listeners
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameInfo.Instance.UnstageCombatInfo();
            if (combatInfo is null)
                return;

            Initialize(combatInfo.players, combatInfo.enemies);
            this.listener = null;
        }
        #endregion

        #region Private Methods
        private void Initialize(List<CharacterStats> players, List<CharacterStats> enemies)
        {
            this.combatGrid.Setup(players, enemies);

            this.textboxControl.Textbox.Message("Enemies approach.", () => BeginCombat());
            UIControlsManager.Instance.TakeControl(this.textboxControl);
        }

        private void BeginCombat()
        {
            UIControlsManager.Instance.TakeControl(this.mainMenu);
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
