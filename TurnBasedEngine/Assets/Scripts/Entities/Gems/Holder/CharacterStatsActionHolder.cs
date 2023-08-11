using System;
using UnityEngine;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class CharacterStatsActionHolder : UtilityEntityHolder<CharacterStatsActionInfo>, ICharacterStatsActionHolder
    {
        public void Destroy(CharacterStatsActionInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[CharacterStatsActionHolder:Destroy] Tried to remove a gem from a bag but the info given was null");
                return;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[CharacterStatsActionHolder:Destroy] Tried to remove a gem from a bag but the info given wasn't in the bag");
                return;
            }

            if (info.Decrement() < 1)
            {
                RemoveAndForget(info);

                if (info.Generated)
                    GameCtx.One.DeleteGemIfCustom(info.ID);
            }
        }

        public void Destroy(string id) => Destroy(Get(id));
    }
}