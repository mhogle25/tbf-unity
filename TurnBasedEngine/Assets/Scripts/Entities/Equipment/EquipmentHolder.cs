using System.Collections.Generic;
using System;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class EquipmentHolder : List<EquipmentInfo>, IEquipmentHolder
    {
        public EquipmentInfo AcquireEquipment(string id)
        {
            EquipmentInfo info = AddEquipment(id);

            if (info is null)
            {
                Debug.LogError($"[EquipmentHolder:AcquireEquipment] Tried to add an equipment with id {id} to an equipment bag but the id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public EquipmentInfo RemoveEquipment(EquipmentInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:RemoveItem] Tried to remove an equipment from an equipment bag but the equipment info given was null");
                return null;
            }

            Remove(info);
            return info;
        }

        private EquipmentInfo AddEquipment(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            foreach (EquipmentInfo info in this)
            {
                if (info.ID == id)
                    return info;
            }

            EquipmentInfo newInfo = new(id);
            Add(newInfo);
            return newInfo;
        }
    }
}