using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    public class EquipModHolder : UtilityEntityHolder<EquipModInfo>, IEquipModHolder
    {
        public void Destroy(EquipModInfo info)
        {
            const string destroyTag = "[EquipModHolder:Destroy]";

            if (info is null)
            {
                Debug.LogError($"{destroyTag} Tried to remove a rune from a bag but the info given was null");
                return;
            }

            if (!Contains(info))
            {
                Debug.LogError($"{destroyTag} Tried to remove a rune from a bag but the info given wasn't in the bag");
                return;
            }

            if (info.Decrement() < 1)
            {
                RemoveAndForget(info);

                if (info.Generated)
                    GameCtx.One.DeleteRuneIfCustom(info.ID);
            }
        }

        public void Destroy(string id) => Destroy(Get(id));
    }
}
