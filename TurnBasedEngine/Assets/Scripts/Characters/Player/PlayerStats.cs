using System;
using Newtonsoft.Json;
using BF2D.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class PlayerStats : CharacterStats
    {
        [JsonIgnore] public override CharacterType Type { get { return CharacterType.Player; } }
        [JsonIgnore] public bool Active = true;
        [JsonIgnore] public int Experience { get { return this.experience; } }
        [JsonProperty] private int experience = 0;
        [JsonIgnore] public int Level { get { return this.level; } }
        [JsonProperty] private int level = 1;
    }
}
