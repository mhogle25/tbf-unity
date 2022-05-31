using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class PlayerStats : Stats
    {
        [JsonIgnore] public Job Profession { get { return this.profession; } }
        [JsonProperty] private Job profession = null;
        [JsonIgnore] public int Experience { get { return this.experience; } }
        [JsonProperty] private int experience = 0;
        [JsonIgnore] public int Level { get { return this.level; } }
        [JsonProperty] private int level = 0;
    }
}
