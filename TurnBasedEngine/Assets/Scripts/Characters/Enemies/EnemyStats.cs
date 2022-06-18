using System;
using Newtonsoft.Json;
using BF2D.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class EnemyStats : CharacterStats
    {
        [JsonIgnore] public override CharacterType Type { get { return CharacterType.Enemy; } }
    }
}
