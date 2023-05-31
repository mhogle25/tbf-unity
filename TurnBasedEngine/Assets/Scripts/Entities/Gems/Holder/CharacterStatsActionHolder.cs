using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game.Actions
{
    public class CharacterStatsActionHolder : List<CharacterStatsActionInfo>, ICharacterStatsActionHolder
    {
        public CharacterStatsActionInfo AcquireGem(string id)
        {
            CharacterStatsActionInfo info = AddGem(id);

            if (info is null)
            {
                Debug.LogError($"[CharacterStatsActionHolder:AcquireGem] Tried to add a gem with id {id} to a gem bag but the gem id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public CharacterStatsActionInfo RemoveGem(CharacterStatsActionInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[CharacterStatsActionHolder:RemoveGem] Tried to remove a gem from a gem bag but the gem info given was null");
                return null;
            }

            Remove(info);
            return info;
        }

        public CharacterStatsActionInfo GetGem(string id)
        {
            foreach (CharacterStatsActionInfo info in this)
            {
                if (info.ID == id)
                    return info;
            }

            return null;
        }

        private CharacterStatsActionInfo AddGem(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            foreach (CharacterStatsActionInfo info in this)
            {
                if (info.ID == id)
                    return info;
            }

            CharacterStatsActionInfo newInfo = new(id);
            Add(newInfo);
            return newInfo;
        }
    }
}