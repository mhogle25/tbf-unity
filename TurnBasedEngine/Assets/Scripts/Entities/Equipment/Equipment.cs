using BF2D.Game.Enums;
using Newtonsoft.Json;
using System;

namespace BF2D.Game
{
    [Serializable]
    public class Equipment : PersistentEffect, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID => this.spriteID;
        [JsonIgnore] public EquipmentType Type => this.equipmentType;

        [JsonProperty] private readonly string spriteID = string.Empty;
        [JsonProperty] private readonly EquipmentType equipmentType = EquipmentType.Accessory;
    }
}
