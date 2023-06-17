using BF2D.Enums;
using Newtonsoft.Json;
using System;
using BF2D.Utilities;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class TargetedCharacterStatsAction : ICombatAligned
    {
        [JsonIgnore] public CharacterTarget Target => this.target;
        [JsonIgnore] public string Description => this.description.Wash();
        [JsonIgnore] public CharacterStatsAction Gem => GameCtx.Instance.GetGem(this.gemID);

        [JsonProperty] private readonly CharacterTarget target = CharacterTarget.Self;
        [JsonProperty] private readonly string description = "target";
        [JsonProperty] private string gemID = string.Empty;

        [JsonIgnore] public CharacterTargetInfo TargetInfo { get => this.targetInfo; }
        [JsonIgnore] private readonly CharacterTargetInfo targetInfo = new();

        [JsonIgnore] public bool CombatExclusive
        {
            get =>
                this.Target == CharacterTarget.Opponent ||
                this.Target == CharacterTarget.AllOpponents ||
                this.Target == CharacterTarget.RandomOpponent;
        }

        [JsonIgnore]
        public bool AutoTargetable
        {
            get =>
                this.Target == CharacterTarget.Self ||
                this.Target == CharacterTarget.All ||
                this.Target == CharacterTarget.AllAllies ||
                this.Target == CharacterTarget.AllOpponents ||
                this.Target == CharacterTarget.Random ||
                this.Target == CharacterTarget.RandomAlly ||
                this.Target == CharacterTarget.RandomOpponent;
        }

        [JsonIgnore] public CombatAlignment Alignment => this.Gem?.Alignment ?? CombatAlignment.Neutral;

        public void SetGemID(string newId) => this.gemID = newId;
    }
}
