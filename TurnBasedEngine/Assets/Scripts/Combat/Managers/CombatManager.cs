﻿using UnityEngine;
using System.Collections.Generic;
using BF2D.Game;
using System;
using BF2D.UI;
using BF2D.Combat.Actions;
using BF2D.Game.Actions;

namespace BF2D.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public class InitializeInfo
        {
            public List<CharacterStats> players = new();
            public List<CharacterStats> enemies = new();
            public string openingMessage = "Enemies approach.";
        }

        [SerializeField] private OptionsGridControlInit mainMenu = null;

        [SerializeField] private DialogTextboxControl standaloneTextboxControl = null;
        [SerializeField] private DialogTextbox orphanedTextbox = null;

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

        public void RunCombat()
        {
            this.CurrentCharacter.UpkeepInit();
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

        #region States
        #endregion

        #region Private Methods
        private void Initialize(InitializeInfo initInfo)
        {
            this.combatGrid.Setup(initInfo.players, initInfo.enemies);

            this.standaloneTextboxControl.Textbox.Message(initInfo.openingMessage, () => UIControlsManager.Instance.TakeControl(this.mainMenu));
            UIControlsManager.Instance.TakeControl(this.standaloneTextboxControl);
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
