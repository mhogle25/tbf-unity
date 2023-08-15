using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
                    GameCtx.One.DeleteEquipmentIfCustom(info.ID);
            }
        }

        public void Destroy(string id) => Destroy(Get(id));

        public IEnumerable<EquipmentInfo> FilterByType(Enums.EquipmentType type) => this.Where(equipment => equipment.Type == type);
    }
}