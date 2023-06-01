using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class CharacterStatsActionHolder : UtilityEntityHolder<CharacterStatsActionInfo>, ICharacterStatsActionHolder
    {
        public string Extract(CharacterStatsActionInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[CharacterStatsActionHolder:RemoveGem] Tried to remove a gem from a gem bag but the gem info given was null");
                return null;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[CharacterStatsActionHolder:RemoveGem] Tried to remove a gem from a gem bag but the gem info given wasn't in the bag");
                return null;
            }

            if (info.Decrement() < 1)
                Remove(info);

            return info.ID;
        }
    }
}