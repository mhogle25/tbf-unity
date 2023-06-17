using System;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentHolder : UtilityEntityHolder<EquipmentInfo>, IEquipmentHolder
    {
        public void Destroy(EquipmentInfo info)
        {
            const string destroyTag = "[EquipmentHolder:Destroy]";

            if (info is null)
            {
                Debug.LogError($"{destroyTag} Tried to remove an equipment from a bag but the info given was null");
                return;
            }

            if (!Contains(info))
            {
                Debug.LogError($"{destroyTag} Tried to remove an equipment from a bag but the info given wasn't in the bag");
                return;
            }

            if (info.Decrement() < 1)
            {
                RemoveAndForget(info);

                if (info.Generated)
                    GameCtx.Instance.DeleteEquipmentIfCustom(info.ID);
            }
        }

        public void Destroy(string id) => Destroy(Get(id));
    }
}