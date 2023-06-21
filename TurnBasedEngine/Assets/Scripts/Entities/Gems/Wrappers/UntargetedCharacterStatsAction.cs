using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class UntargetedCharacterStatsAction : ICombatAligned
    {
        [JsonProperty] private string gemID = string.Empty;
        [JsonIgnore] private CharacterStatsAction Gem => GameCtx.Instance.GetGem(this.gemID);

        [JsonIgnore] public CombatAlignment Alignment => this.Gem?.Alignment ?? CombatAlignment.Neutral;

        [JsonIgnore] public string ID => this.Gem.ID;

        public void SetGemID(string newId) => this.gemID = newId;

        public string GetAnimationKey() => this.Gem.GetAnimationKey();

        public CharacterStatsAction.Info Run(CharacterStats self) => this.Gem.Run(self, self);
    }
}
