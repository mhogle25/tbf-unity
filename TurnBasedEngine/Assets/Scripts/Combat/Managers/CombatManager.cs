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
            public float themePaletteOffset = 0.26f;
        }

        [Header("UI")]
        [SerializeField] private OptionsGridControlInit mainMenu = null;

        [SerializeField] private DialogTextboxControl standaloneTextboxControl = null;
        [SerializeField] private DialogTextbox orphanedTextbox = null;

        [SerializeField] private CombatGrid combatGrid = null;
        [SerializeField] private CharacterTargeterControl characterTargeter = null;

        [Header("VFX")]
        [SerializeField] private Utilities.MaterialController themeController;

        private Action listener = null;

        private static CombatManager instance = null;
        public static CombatManager Instance => CombatManager.instance;

        public CharacterCombat CurrentCharacter => this.combatGrid.CurrentCharacter; 
        public IEnumerable<CharacterCombat> Players =>  this.combatGrid.ActivePlayers;  
        public int PlayerCount => this.combatGrid.ActivePlayers.Length; 
        public IEnumerable<CharacterCombat> Enemies => this.combatGrid.ActiveEnemies; 
        public int EnemyCount => this.combatGrid.ActiveEnemies.Length; 
        public IEnumerable<CharacterCombat> Characters => this.combatGrid.ActiveCharacters; 
        public int CharacterCount => this.combatGrid.ActivePlayers.Length + this.combatGrid.ActiveEnemies.Length; 

        public DialogTextbox OrphanedTextbox => this.orphanedTextbox; 

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
            this.CurrentCharacter.SetupCombatAction(this.characterTargeter, new CombatAction
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
            this.CurrentCharacter.Run();
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
                this.standaloneTextboxControl.Dialog("di_victory", 0);
                List<JobInfo.LevelUpInfo> infos = AllocateExperience(this.Players, this.combatGrid.GetTotalExperience());
                foreach (JobInfo.LevelUpInfo info in infos)
                {
                    this.standaloneTextboxControl.Dialog(info.levelUpDialog, 0, null, new string[]
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
                    this.standaloneTextboxControl.Message(itemLootMessage);

                // Equipment Loot
                string equipmentLootMessage = string.Empty;
                foreach (string id in this.combatGrid.GetTotalEquipmentLoot())
                {
                    EquipmentInfo equipmentInfo = GameCtx.Instance.PartyEquipments.Acquire(id);
                    if (equipmentInfo is not null)
                        equipmentLootMessage += $"Acquired a {equipmentInfo.Get().Name}. {Strings.DialogTextbox.BriefPause}";
                }

                if (!string.IsNullOrEmpty(equipmentLootMessage))
                    this.standaloneTextboxControl.Message(equipmentLootMessage);

                // Gem Loot
                string gemLootMessage = string.Empty;
                foreach (string id in this.combatGrid.GetTotalGemLoot())
                {
                    CharacterStatsActionInfo gemInfo = GameCtx.Instance.PartyGems.Acquire(id);
                    if (gemInfo is not null)
                        gemLootMessage += $"Acquired a {gemInfo.Get().Name}. {Strings.DialogTextbox.BriefPause}";
                }

                if (!string.IsNullOrEmpty(gemLootMessage))
                    this.standaloneTextboxControl.Message(gemLootMessage);


                // Point Loot
                GameCtx.Instance.Currency += this.combatGrid.GetTotalCurrencyLoot();
                int etherUp = this.combatGrid.GetTotalEtherLoot();
                GameCtx.Instance.Ether += etherUp;

                string pointLootMessage = $"The party obtained {this.combatGrid.GetTotalCurrencyLoot()} {Strings.Game.Currency}";
                if (etherUp > 0)
                    pointLootMessage += $" and {etherUp} {Strings.Game.Ether}.";
                else
                    pointLootMessage += '.';
                this.standaloneTextboxControl.Message(pointLootMessage, () =>
                {
                    GameCtx.Instance.SaveGame();
                    CancelCombat();
                });
            }
            else if (PlayersAreDefeated())
            {
                this.standaloneTextboxControl.Dialog("di_defeat", 0, CancelCombat);
            }
            else
            {
                this.standaloneTextboxControl.Dialog("di_draw", 0, CancelCombat);
            }

            this.standaloneTextboxControl.TakeControl();
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

            this.themeController.NewPaletteOffsetClocked(initInfo.themePaletteOffset);
            this.standaloneTextboxControl.Dialog(initInfo.openingDialogKey, 0, BeginTurn);
            this.standaloneTextboxControl.TakeControl();

            Terminal.IO.Log($"Combat intialized with {CombatManager.Instance.PlayerCount} players and {CombatManager.Instance.EnemyCount} enemies.");
        }

        private void BeginTurn()
        {
            if (this.combatGrid.CharacterIsPlayer(this.CurrentCharacter))
            {
                this.standaloneTextboxControl.Message($"{this.CurrentCharacter.Stats.Name}'s turn. {Strings.DialogTextbox.BriefPause}(Level {this.CurrentCharacter.Stats.Level} {this.CurrentCharacter.Stats.CurrentJob.Name})", () =>
                {
                    UIControlsManager.Instance.TakeControl(this.mainMenu);
                });
                this.standaloneTextboxControl.TakeControl();
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
