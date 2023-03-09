using BF2D.Game.Enums;
using Newtonsoft.Json;

namespace BF2D.Game
{
    public class Equipment : PersistentEffect, IUtilityEntity
    {
        [JsonIgnore] public string SpriteID { get { return this.spriteID; } }
        [JsonProperty] private readonly string spriteID = string.Empty;
        [JsonIgnore] public EquipmentType Type { get { return this.equipmentType; } }
        [JsonProperty] private readonly EquipmentType equipmentType = EquipmentType.Accessory;
        [JsonIgnore] public CombatAlignment Alignment { get { return this.alignment; } }
        [JsonProperty] private readonly CombatAlignment alignment = CombatAlignment.Defense;
    }
}
