using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public class PersistentEffect : Entity
    {
        [JsonIgnore] public int SpeedModifier { get => this.speedModifier; }
        [JsonProperty] private readonly int speedModifier = 0;
        [JsonIgnore] public int AttackModifier { get => this.attackModifier; }
        [JsonProperty] private readonly int attackModifier = 0;
        [JsonIgnore] public int DefenseModifier { get => this.defenseModifier; }
        [JsonProperty] private readonly int defenseModifier = 0;
        [JsonIgnore] public int FocusModifier { get => this.focusModifier; }
        [JsonProperty] private readonly int focusModifier = 0;
        [JsonIgnore] public int LuckModifier { get => this.luckModifier; }
        [JsonProperty] private readonly int luckModifier = 0;
        [JsonIgnore] public int MaxHealthModifier { get => this.maxHealthModifier; }
        [JsonProperty] private readonly int maxHealthModifier = 0;
        [JsonIgnore] public int MaxStaminaModifier { get => this.maxStaminaModifier; }
        [JsonProperty] private readonly int maxStaminaModifier = 0;
        [JsonIgnore] public UntargetedGameAction OnUpkeep { get => this.onUpkeep; }
        [JsonProperty] private readonly UntargetedGameAction onUpkeep = null;
        [JsonIgnore] public UntargetedGameAction OnEOT { get => this.onEOT; }
        [JsonProperty] private readonly UntargetedGameAction onEOT = null;

        public bool UpkeepEventExists()
        {
            return this.onUpkeep is not null;
        }

        public bool EOTEventExists()
        {
            return this.onEOT is not null;
        }

        public int GetModifier(CharacterStatsProperty property)
        {
            return property switch
            {
                CharacterStatsProperty.Speed => this.speedModifier,
                CharacterStatsProperty.Attack => this.attackModifier,
                CharacterStatsProperty.Defense => this.defenseModifier,
                CharacterStatsProperty.Focus => this.focusModifier,
                CharacterStatsProperty.Luck => this.luckModifier,
                CharacterStatsProperty.MaxHealth => this.maxHealthModifier,
                CharacterStatsProperty.MaxStamina => this.maxStaminaModifier,
                _ => 0
            };
        }
    }
}