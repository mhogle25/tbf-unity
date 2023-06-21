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

        [JsonProperty] private readonly CharacterTarget target = CharacterTarget.Self;
        [JsonProperty] private readonly string description = "target";
        [JsonProperty] private string gemID = string.Empty;
        [JsonIgnore] private CharacterStatsAction Gem => GameCtx.Instance.GetGem(this.gemID);

        [JsonIgnore] public CharacterTarget Target => this.target;
        [JsonIgnore] public string Description => this.description.Wash();

        [JsonIgnore] public CharacterTargetInfo TargetInfo => this.targetInfo;
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

        public bool ContainsAura(AuraType aura) => this.Gem.ContainsAura(aura);

        public string GetAnimationKey() => this.Gem.GetAnimationKey();

        public CharacterStatsAction.Info Run(CharacterStats source, CharacterStats target) => this.Gem.Run(source, target);

        public string TextBreakdown(CharacterStats source) => this.Gem.TextBreakdown(source);

        public void SetGemID(string newId) => this.gemID = newId;
    }
}
