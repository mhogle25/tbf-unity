using UnityEngine;
using System.Collections.Generic;
using System;
using BF2D.UI;
using BF2D.Game.Combat.Actions;
using BF2D.Game.Actions;

namespace BF2D.Game.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public class InitializeInfo
        {
            public IEnumerable<CharacterStats> players = null;
            public IEnumerable<CharacterStats> enemies = null;
            public string openingDialogKey = $"di_opening_{Strings.System.Default}";
        }

        [SerializeField] private OptionsGridControlInit mainMenu = null;

        [SerializeField] private DialogTextboxControl standaloneTextboxControl = null;
        [SerializeField] private DialogTextbox orphanedTextbox = null;

        [SerializeField] private CombatGrid combatGrid = null;
        [SerializeField] private CharacterTargeterControl characterTargeter = null;

        private Action listener = null;

        private static CombatManager instance = null;
        public static CombatManager Instance => CombatManager.instance;

        public CharacterCombat CurrentCharacter { get => this.combatGrid.CurrentCharacter; }
        public IEnumerable<CharacterCombat> Players { get =>  this.combatGrid.ActivePlayers;  }
        public int PlayerCount { get => this.combatGrid.ActivePlayers.Length; }
        public IEnumerable<CharacterCombat> Enemies { get => this.combatGrid.ActiveEnemies; }
        public int EnemyCount { get => this.combatGrid.ActiveEnemies.Length; }
        public IEnumerable<CharacterCombat> Characters { get => this.combatGrid.ActiveCharacters; }
        public int CharacterCount { get => this.combatGrid.ActivePlayers.Length + this.combatGrid.ActiveEnemies.Length; }

        public DialogTextbox OrphanedTextbox { get => this.orphanedTextbox; }

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
            },
            this.characterTargeter
            );
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

            if (EnemiesAreDefeated())
            {
                this.standaloneTextboxControl.Textbox.Dialog("di_victory", false, 0);
                List<JobInfo.LevelUpInfo> infos = AllocateExperience(this.Players, this.combatGrid.GetTotalExperience());
                foreach (JobInfo.LevelUpInfo info in infos)
                {
                    this.standaloneTextboxControl.Textbox.Dialog(info.levelUpDialog, false, 0, null, new string[]
                    {
                        info.parent.Name
                    });
                }

                // Item Loot
                string itemLootMessage = string.Empty;
                foreach (string id in this.combatGrid.GetTotalItemLoot())
                {
                    ItemInfo itemInfo = GameCtx.Instance.PartyItems.Acquire(id);
                    if (itemInfo is not null)
                        itemLootMessage += $"Acquired a {itemInfo.Get().Name}. {Strings.DialogTextbox.BriefPause}";
                }

                if (!string.IsNullOrEmpty(itemLootMessage))
                    this.standaloneTextboxControl.Textbox.Message(itemLootMessage, false);

                // Equipment Loot
                string equipmentLootMessage = string.Empty;
                foreach (string id in this.combatGrid.GetTotalEquipmentLoot())
                {
                    EquipmentInfo equipmentInfo = GameCtx.Instance.PartyEquipments.Acquire(id);
                    if (equipmentInfo is not null)
                        equipmentLootMessage += $"Acquired a {equipmentInfo.Get().Name}. {Strings.DialogTextbox.BriefPause}";
                }

                if (!string.IsNullOrEmpty(equipmentLootMessage))
                    this.standaloneTextboxControl.Textbox.Message(equipmentLootMessage, false);

                // Gem Loot
                string gemLootMessage = string.Empty;
                foreach (string id in this.combatGrid.GetTotalGemLoot())
                {
                    CharacterStatsActionInfo gemInfo = GameCtx.Instance.PartyGems.Acquire(id);
                    if (gemInfo is not null)
                        gemLootMessage += $"Acquired a {gemInfo.Get().Name}. {Strings.DialogTextbox.BriefPause}";
                }

                if (!string.IsNullOrEmpty(gemLootMessage))
                    this.standaloneTextboxControl.Textbox.Message(gemLootMessage, false);


                // Point Loot
                GameCtx.Instance.Currency += this.combatGrid.GetTotalCurrencyLoot();
                int etherUp = this.combatGrid.GetTotalEtherLoot();
                GameCtx.Instance.Ether += etherUp;

                string pointLootMessage = $"The party obtained {this.combatGrid.GetTotalCurrencyLoot()} {Strings.Game.Currency}";
                if (etherUp > 0)
                    pointLootMessage += $" and {etherUp} {Strings.Game.Ether}.";
                else
                    pointLootMessage += '.';
                this.standaloneTextboxControl.Textbox.Message(pointLootMessage, false, () =>
                {
                    GameCtx.Instance.SaveGame();
                    CancelCombat();
                });
            }
            else if (PlayersAreDefeated())
            {
                this.standaloneTextboxControl.Textbox.Dialog("di_defeat", false, 0, CancelCombat);
            }
            else
            {
                this.standaloneTextboxControl.Textbox.Dialog("di_draw", false, 0, CancelCombat);
            }

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
                enemiesDefeated = enemiesDefeated && enemy.Stats.Dead;
            return enemiesDefeated;
        }

        public bool PlayersAreDefeated()
        {
            bool playersDefeated = true;
            foreach (CharacterCombat player in this.Players)
                playersDefeated = playersDefeated && player.Stats.Dead;
            return playersDefeated;
        }

        public bool PlayersAreAtFullHealth() => CharactersAreAtFullHealth(this.Players);

        public bool EnemiesAreAtFullHealth() => CharactersAreAtFullHealth(this.Enemies);

        public CharacterCombat RandomCharacter()
        {
            return this.combatGrid.ActiveCharacters[UnityEngine.Random.Range(0, this.combatGrid.ActiveCharacters.Length)];
        }

        public CharacterCombat RandomEnemy()
        {
            return this.combatGrid.ActiveEnemies[UnityEngine.Random.Range(0, this.combatGrid.ActiveEnemies.Length)];
        }

        public CharacterCombat RandomPlayer()
        {
            return this.combatGrid.ActivePlayers[UnityEngine.Random.Range(0, this.combatGrid.ActivePlayers.Length)];
        }

        public bool CharacterIsPlayer(CharacterCombat character)
        {
            return this.combatGrid.CharacterIsPlayer(character);
        }

        public bool CharacterIsEnemy(CharacterCombat character)
        {
            return this.combatGrid.CharacterIsEnemy(character);
        }
        #endregion

        #region Listeners
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameCtx.Instance.UnstageCombatInfo();
            if (combatInfo is null)
                return;

            Initialize(combatInfo);
            this.listener = null;
        }
        #endregion

        #region Private Methods
        private bool CharactersAreAtFullHealth(IEnumerable<CharacterCombat> characters)
        {
            foreach (CharacterCombat character in characters)
                if (character.Stats.Health != character.Stats.MaxHealth && !character.Stats.Dead)
                    return false;
            return true;
        }

        private void Initialize(InitializeInfo initInfo)
        {
            this.combatGrid.Setup(initInfo.players, initInfo.enemies);

            this.standaloneTextboxControl.Textbox.Dialog(initInfo.openingDialogKey, false, 0, BeginTurn);
            UIControlsManager.Instance.TakeControl(this.standaloneTextboxControl);

            Terminal.IO.Log($"Combat intialized with {CombatManager.Instance.PlayerCount} players and {CombatManager.Instance.EnemyCount} enemies.");
        }

        private void BeginTurn()
        {
            if (this.combatGrid.CharacterIsPlayer(this.CurrentCharacter))
            {
                this.standaloneTextboxControl.Textbox.Message($"{this.CurrentCharacter.Stats.Name}'s turn. {Strings.DialogTextbox.BriefPause}(Level {this.CurrentCharacter.Stats.Level} {this.CurrentCharacter.Stats.CurrentJob.Name})", false, () =>
                {
                    UIControlsManager.Instance.TakeControl(this.mainMenu);
                });
                UIControlsManager.Instance.TakeControl(this.standaloneTextboxControl);
                return;
            }

            this.CurrentCharacter.Stats.CombatAI.Run();
        }

        private void SingletonSetup()
        {
            //Setup of Monobehaviour Singleton
            if (CombatManager.instance && CombatManager.instance != this)
                Destroy(CombatManager.instance.gameObject);

            CombatManager.instance = this;
        }

        private List<JobInfo.LevelUpInfo> AllocateExperience(IEnumerable<CharacterCombat> characters, long experience)
        {
            List<JobInfo.LevelUpInfo> infos = new();

            foreach (CharacterCombat character in characters)
            {
                JobInfo.LevelUpInfo info = character.Stats.GrantExperience(experience);
                if (info.leveledUp)
                    infos.Add(info);
            }

            return infos;
        }
        #endregion
    }
}
