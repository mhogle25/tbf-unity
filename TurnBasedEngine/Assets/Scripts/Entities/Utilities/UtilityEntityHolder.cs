using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    public class UtilityEntityHolder<T> : List<T> where T : UtilityEntityInfo, new()
    {
        public T Acquire(string id)
        {
            T info = AddIfNone(id);

            if (info is null)
            {
                Debug.LogError($"[UtilityEntityHolder:Acquire] Tried to add an entity with id {id} to a bag but the id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public T Transfer(T info, IUtilityEntityHolder<T> receiver)
        {
            if (info is null)
            {
                Debug.LogError($"[UtilityEntityHolder:Transfer] Tried to transfer an entity from a bag but the info given was null");
                return null;
            }

            if (!Contains(info))
            {
                Debug.LogError($"[UtilityEntityHolder:Transfer] Tried to transfer an entity from a bag but the info given wasn't in the bag");
                return null;
            }

            if (info.Decrement() < 1)
                Remove(info);

            return receiver.Acquire(info.ID);
        }

        public T Get(string id)
        {
            foreach (T info in this)
            {
                if (info.ID == id)
                    return info;
            }

            return null;
        }

        protected T AddIfNone(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            foreach (T info in this)
            {
                if (info.ID == id)
                    return info;
            }

            T newInfo = new()
            {
                ID = id
            };
            Add(newInfo);
            return newInfo;
        }
    }
}