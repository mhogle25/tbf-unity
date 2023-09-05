using System;
using BF2D.Game.Actions;
using UnityEngine;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EquipMod : PersistentEffect, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID => this.spriteID;
        [JsonProperty] private readonly string spriteID = string.Empty;

        [JsonIgnore] public UntargetedGameAction OnEquip => this.onEquip;
        [JsonProperty] private readonly UntargetedGameAction onEquip = null;

        public Sprite GetIcon() => GameCtx.One.GetIcon(this.SpriteID);
    }
}