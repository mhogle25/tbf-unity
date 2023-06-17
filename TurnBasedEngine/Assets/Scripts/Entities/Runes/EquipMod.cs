using System;
using BF2D.Game.Enums;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class EquipMod : PersistentEffect, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID => this.spriteID;
        [JsonProperty] private readonly string spriteID = string.Empty;

        public Entity GetEntity() => this;
    }
}