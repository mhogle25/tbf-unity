using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    public static class Numbers
    {
        public const float clockSpeed = 0.03125f;
        public const int maxPartySize = 4;
        public const int maxLevel = 99;
        public const int baseCritMultiplier = 2;
        public const int baseCritChance = 5;

        public static bool ValidGridIndex(int index)
        {
            return index >= 0 && index < Numbers.maxPartySize;
        }
    }
}
