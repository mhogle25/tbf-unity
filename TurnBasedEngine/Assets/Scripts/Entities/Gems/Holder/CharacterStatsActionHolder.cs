using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D.Game.Actions
{
    [Serializable]
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

        public string ExtractGem(CharacterStatsActionInfo info)
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

        public CharacterStatsActionInfo TransferGem(CharacterStatsActionInfo info, ICharacterStatsActionHolder receiver)
        {
            if (info is null)
            {
                Debug.LogError($"[CharacterStatsActionHolder:TransferGem] Tried to transfer a gem from a gem bag but the gem info given was null");
                return null;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[CharacterStatsActionHolder:TransferGem] Tried to transfer a gem from a gem bag but the gem info given wasn't in the bag");
                return null;
            }

            if (info.Decrement() < 1)
                Remove(info);

            return receiver.AcquireGem(info.ID);
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