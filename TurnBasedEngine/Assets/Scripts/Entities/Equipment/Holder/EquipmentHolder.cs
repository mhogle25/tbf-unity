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

        public void RemoveEquipment(EquipmentInfo info)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:RemoveEquipment] Tried to remove an equipment from an equipment bag but the equipment info given was null");
                return;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[ItemHolder:RemoveEquipment] Tried to remove an equipment from an equipment bag but the equipment info given wasn't in the bag");
                return;
            }

            if (info.Decrement() < 1)
            {
                Remove(info);

                if (info.Generated)
                    GameCtx.Instance.DeleteEquipmentIfCustom(info.ID);
            }
        }

        public EquipmentInfo TransferEquipment(EquipmentInfo info, IEquipmentHolder receiver)
        {
            if (info is null)
            {
                Debug.LogError($"[ItemHolder:TransferEquipment] Tried to transfer an equipment from an item bag but the equipment info given was null");
                return null;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[ItemHolder:TransferEquipment] Tried to transfer an equipment from an equipment bag but the equipment info given wasn't in the bag");
                return null;
            }

            if (info.Decrement() < 1)
                Remove(info);

            return receiver.AcquireEquipment(info.ID);
        }

        public string ExtractEquipment(EquipmentInfo info)
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

        public EquipmentInfo GetEquipment(string id)
        {
            foreach (EquipmentInfo info in this)
            {
                if (info.ID == id)
                    return info;
            }

            return null;
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