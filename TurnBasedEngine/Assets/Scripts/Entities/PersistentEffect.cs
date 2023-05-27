using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Game.Actions;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    public class PersistentEffect : Entity, ICombatAligned
    {
        [JsonIgnore] public virtual int SpeedModifier => this.speedModifier;
        [JsonIgnore] public virtual int AttackModifier => this.attackModifier;
        [JsonIgnore] public virtual int DefenseModifier => this.defenseModifier;
        [JsonIgnore] public virtual int FocusModifier => this.focusModifier; 
        [JsonIgnore] public virtual int LuckModifier => this.luckModifier; 
        [JsonIgnore] public virtual int MaxHealthModifier => this.maxHealthModifier; 
        [JsonIgnore] public virtual int MaxStaminaModifier => this.maxStaminaModifier; 
        [JsonIgnore] public virtual UntargetedGameAction OnUpkeep => this.onUpkeep; 
        [JsonIgnore] public virtual UntargetedGameAction OnEOT => this.onEOT;

        [JsonIgnore] public virtual CombatAlignment Alignment
        {
            get
            {
                int offensePoints = 0;
                int defensePoints = 0;
                int neutralPoints = 0;

                neutralPoints += this.SpeedModifier;
                offensePoints += this.AttackModifier;
                defensePoints += this.DefenseModifier;
                neutralPoints += this.FocusModifier;
                neutralPoints += this.LuckModifier;
                defensePoints += this.MaxHealthModifier;
                neutralPoints += this.MaxStaminaModifier;

                return CombatAlignmentSelector.Calculate(offensePoints, defensePoints, neutralPoints);
            }
        }

        [JsonProperty] private readonly int speedModifier = 0;
        [JsonProperty] private readonly int attackModifier = 0;
        [JsonProperty] private readonly int defenseModifier = 0;
        [JsonProperty] private readonly int focusModifier = 0;
        [JsonProperty] private readonly int luckModifier = 0;
        [JsonProperty] private readonly int maxHealthModifier = 0;
        [JsonProperty] private readonly int maxStaminaModifier = 0;
        [JsonProperty] private readonly UntargetedGameAction onUpkeep = null;
        [JsonProperty] private readonly UntargetedGameAction onEOT = null;

        public bool UpkeepEventExists() => this.onUpkeep is not null;

        public bool EOTEventExists() => this.onEOT is not null;

        public int GetModifier(CharacterStatsProperty property)
        {
            return property switch
            {
                CharacterStatsProperty.Speed => this.SpeedModifier,
                CharacterStatsProperty.Attack => this.AttackModifier,
                CharacterStatsProperty.Defense => this.DefenseModifier,
                CharacterStatsProperty.Focus => this.FocusModifier,
                CharacterStatsProperty.Luck => this.LuckModifier,
                CharacterStatsProperty.MaxHealth => this.MaxHealthModifier,
                CharacterStatsProperty.MaxStamina => this.MaxStaminaModifier,
                _ => 0
            };
        }
    }
}