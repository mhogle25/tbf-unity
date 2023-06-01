using System.Collections.Generic;
using System;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentHolder : UtilityEntityHolder<EquipmentInfo>, IEquipmentHolder
    {
        public void Destroy(EquipmentInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:DestroyEquipment] Tried to remove an equipment from an equipment bag but the equipment info given was null");
                return;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[ItemHolder:DestroyEquipment] Tried to remove an equipment from an equipment bag but the equipment info given wasn't in the bag");
                return;
            }

            if (info.Decrement() < 1)
            {
                Remove(info);

                if (info.Generated)
                    GameCtx.Instance.DeleteEquipmentIfCustom(info.ID);
            }
        }

        public string Extract(EquipmentInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:ExtractEquipment] Tried to extract an equipment from an item bag but the equipment info given was null");
                return string.Empty;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[ItemHolder:ExtractEquipment] Tried to extract an equipment from an equipment bag but the equipment info given wasn't in the bag");
                return string.Empty;
            }

            if (info.Decrement() < 1)
                Remove(info);

            return info.ID;
        }
    }
}