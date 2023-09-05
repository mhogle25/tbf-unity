using UnityEngine;
using System.Collections.Generic;
using System;
using BF2D.UI;
using BF2D.Game.Combat.Actions;
using BF2D.Game.Enums;

namespace BF2D.Game.Combat
{
    public class CombatCtx : MonoBehaviourSingleton<CombatCtx>
    {
        public class InitializeInfo
        {
            public Party players = null;
            public Encounter enemies = null;
            public string openingDialogKey = $"di_opening_{Strings.System.DEFAULT_ID}";
            public float themePaletteOffset = 0.3f;
        }

        [Header("UI")]
        [SerializeField] private OptionsGridControlInit mainMenu = null;

        [SerializeField] private DialogTextboxControl standaloneTextboxControl = null;
        [SerializeField] private DialogTextbox orphanedTextbox = null;

        [SerializeField] private CombatGrid combatGrid = null;
        [SerializeField] private ItemsCharacterTargeter characterTargeter = null;

        [Header("VFX")]
        [SerializeField] private Utilities.MaterialController themeController;

        private Action listener = null;

        public CharacterCombat CurrentCharacter => this.combatGrid.CurrentCharacter;

        public IEnumerable<CharacterCombat> Players => this.combatGrid.ActivePlayers;
        public int PlayerCount => this.combatGrid.ActivePlayers.Length;

        public IEnumerable<CharacterCombat> Enemies => this.combatGrid.ActiveEnemies;
        public int EnemyCount => this.combatGrid.ActiveEnemies.Length;

        public IEnumerable<CharacterCombat> Characters => this.combatGrid.ActiveCharacters;
        public int CharacterCount => this.combatGrid.ActivePlayers.Length + this.combatGrid.ActiveEnemies.Length;

        public IEnumerable<CharacterCombat> Allies => GetAllies(this.CurrentCharacter.Alignment);
        public int AllyCount => GetAllies(this.CurrentCharacter.Alignment).Length;

        public IEnumerable<CharacterCombat> Opponents => GetOpponents(this.CurrentCharacter.Alignment);
        public int OpponentCount => GetOpponents(this.CurrentCharacter.Alignment).Length;

        public IEnumerable<CharacterCombat> AllPlayers => this.combatGrid.AllPlayers;
        public int AllPlayersCount => this.combatGrid.AllPlayers.Length;

        public IEnumerable<CharacterCombat> AllEnemies => this.combatGrid.AllEnemies;
        public int AllEnemiesCount => this.combatGrid.AllEnemies.Length;

        public IEnumerable<CharacterCombat> AllCharacters => this.combatGrid.AllCharacters;
        public int AllCharactersCount => this.combatGrid.AllPlayers.Length + this.combatGrid.AllEnemies.Length;

        public IEnumerable<CharacterCombat> AllAllies => GetAllAllies(this.CurrentCharacter.Alignment);
        public int AllAlliesCount => GetAllAllies(this.CurrentCharacter.Alignment).Length;

        public IEnumerable<CharacterCombat> AllOpponents => GetAllOpponents(this.CurrentCharacter.Alignment);
        public int AllOpponentsCount => GetAllOpponents(this.CurrentCharacter.Alignment).Length;

        public DialogTextbox OrphanedTextbox => this.orphanedTextbox;

        protected sealed override void SingletonAwakened()
        {
            this.listener = CombatInfoListen;
        }

        private void Update()
        {
            this.listener?.Invoke();
        }

        #region Public Utilities
        public void SetupItemCombat(ItemInfo info)
        {
            this.CurrentCharacter.SetupCombatAction(this.characterTargeter, new CombatAction
            {
                Item = new ItemCombatActionInfo
                {
                    Info = info
                }
            });
        }

        public void SetupEquipCombat(EquipmentInfo info, EquipmentType type)
        {
            this.CurrentCharacter.SetupCombatAction(new CombatAction
            {
                Equip = new EquipCombatActionInfo
                {
                    Info = info,
                    Type = type
                }
            });
        }

        public void RunCombatEvents()
        {
            UICtx.One.ResetControlChain(false);
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
                    this.standaloneTextboxControl.Dialog(info.levelUpDialog, 0, null, info.parent.Name);

                // Item Loot
                HandleUtilityEntityLoot(GameCtx.One.Items, this.combatGrid.GetTotalItemLoot());

                // Equipment Loot
                HandleUtilityEntityLoot(GameCtx.One.Equipment, this.combatGrid.GetTotalEquipmentLoot());

                // Gem Loot
                HandleUtilityEntityLoot(GameCtx.One.Gems, this.combatGrid.GetTotalGemLoot());

                // Rune Loot
                HandleUtilityEntityLoot(GameCtx.One.Runes, this.combatGrid.GetTotalRuneLoot());

                // Point Loot
                GameCtx.One.Currency += this.combatGrid.GetTotalCurrencyLoot();
                int etherUp = this.combatGrid.GetTotalEtherLoot();
                GameCtx.One.Ether += etherUp;

                string pointLootMessage = $"The party obtained {this.combatGrid.GetTotalCurrencyLoot()} {Strings.Game.CURRENCY}";
                if (etherUp > 0)
                    pointLootMessage += $" and {etherUp} {Strings.Game.ETHER}.";
                else
                    pointLootMessage += '.';
                this.standaloneTextboxControl.Message(pointLootMessage, () =>
                {
                    GameCtx.One.SaveGame();
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
            UICtx.One.ResetControlChain(false);
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

        public bool OpponentsAreAtFullHealth() => CharactersAreAtFullHealth(this.Opponents);

        public bool AlliesAreAtFullHealth() => CharactersAreAtFullHealth(this.Allies);

        public CharacterCombat RandomCharacter()
        {
            return this.combatGrid.ActiveCharacters[UnityEngine.Random.Range(0, this.combatGrid.ActiveCharacters.Length)];
        }

        public CharacterCombat RandomAlly()
        {
            if (this.AllyCount < 1)
                return null;

            return GetAllies(this.CurrentCharacter.Alignment)[UnityEngine.Random.Range(0, this.AllyCount)];
        }

        public CharacterCombat RandomOpponent()
        {
            if (this.OpponentCount < 1)
                return null;

            return GetOpponents(this.CurrentCharacter.Alignment)[UnityEngine.Random.Range(0, this.OpponentCount)];
        }
        #endregion

        #region Listeners
        private void CombatInfoListen()
        {
            InitializeInfo combatInfo = GameCtx.One.UnstageEncounter();
            if (combatInfo is null)
                return;

            Initialize(combatInfo);
            this.listener = null;
        }
        #endregion

        #region Private Methods
        private void HandleUtilityEntityLoot<T>(IUtilityEntityHolder<T> receiver, IEnumerable<string> ids) where T : UtilityEntityInfo
        {
            string lootMessage = string.Empty;
            foreach (string id in ids)
            {
                T info = receiver.Acquire(id);
                if (info is not null)
                    lootMessage += $"Acquired a {info.Name}. {Strings.DialogTextbox.PAUSE_BREIF}";
            }

            if (!string.IsNullOrEmpty(lootMessage))
                this.standaloneTextboxControl.Message(lootMessage);
        }

        private string NPC_ERROR(string name) => $"[CombatCtx:{name}] Allignment cannot be NPC";

        private CharacterCombat[] GetAllies(CharacterAlignment alignment) => alignment switch
        {
            CharacterAlignment.Player => this.combatGrid.ActivePlayers,
            CharacterAlignment.Enemy => this.combatGrid.ActiveEnemies,
            _ => throw new Exception(NPC_ERROR("GetAllies"))
        };

        private CharacterCombat[] GetOpponents(CharacterAlignment alignment) => alignment switch
        {
            CharacterAlignment.Player => this.combatGrid.ActiveEnemies,
            CharacterAlignment.Enemy => this.combatGrid.ActivePlayers,
            _ => throw new Exception(NPC_ERROR("GetOpponents"))
        };

        private CharacterCombat[] GetAllAllies(CharacterAlignment alignment) => alignment switch
        {
            CharacterAlignment.Player => this.combatGrid.AllPlayers,
            CharacterAlignment.Enemy => this.combatGrid.AllEnemies,
            _ => throw new Exception(NPC_ERROR("GetAllAllies"))
        };

        private CharacterCombat[] GetAllOpponents(CharacterAlignment alignment) => alignment switch
        {
            CharacterAlignment.Player => this.combatGrid.AllEnemies,
            CharacterAlignment.Enemy => this.combatGrid.AllPlayers,
            _ => throw new Exception(NPC_ERROR("GetAllOpponents"))
        };

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

            this.standaloneTextboxControl.Dialog(initInfo.openingDialogKey, 0, BeginTurn);
            this.standaloneTextboxControl.TakeControl();

            ShCtx.One.Log($"Combat intialized with {CombatCtx.One.PlayerCount} players and {CombatCtx.One.EnemyCount} enemies.");
        }

        private void BeginTurn()
        {
            if (this.CurrentCharacter.IsPlayer)
            {
                this.standaloneTextboxControl.Message($"{this.CurrentCharacter.Stats.Name}'s turn. {Strings.DialogTextbox.PAUSE_BREIF}(Level {this.CurrentCharacter.Stats.Level} {this.CurrentCharacter.Stats.CurrentJob.Name})", () =>
                {
                    UICtx.One.TakeControl(this.mainMenu);
                });
                this.standaloneTextboxControl.TakeControl();
                return;
            }

            this.CurrentCharacter.Stats.CombatAI.Run();
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