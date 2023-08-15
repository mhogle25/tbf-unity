 using BF2D.Enums;
using Newtonsoft.Json;
using System;
using BF2D.Utilities;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class TargetedCharacterStatsActionSlot : CharacterStatsActionSlot, ICombatAligned
    {
        [JsonProperty] private readonly CharacterTarget target = CharacterTarget.Self;
        [JsonProperty] private readonly string description = "target";

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

        public CharacterStatsAction.Info Run(CharacterStats source, CharacterStats target) => this.Gem.Run(source, target, this.Specs);
    }
}
