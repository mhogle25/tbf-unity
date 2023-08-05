using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace BF2D.Game
{
    [Serializable]
    public class UtilityEntityHolder<T> : List<T>, IUtilityEntityHolder<T> where T : UtilityEntityInfo, new()
    {
        [JsonIgnore] protected readonly Dictionary<string, T> utilityEntityIndex = new();

        public T Acquire(string id)
        {
            T info = AddIfNone(id);

            if (info is null)
            {
                Debug.LogError($"[UtilityEntityHolder:Acquire] Tried to add an entity with id '{id}' to a bag but the id given was invalid");
                return null;
            }

            info.Increment();
            return info;
        }

        public T Transfer(T info, IUtilityEntityHolder<T> holder)
        {
            if (holder is null)
            {
                Debug.LogError("[UtilityEntityHolder:Transfer] Tried to transfer an entity from a bag but the receiver given was null");
                return null;
            }

            return Acquire(holder.Extract(info));
        }

        public T Transfer(string id, IUtilityEntityHolder<T> holder) => Transfer(Get(id), holder);

        public string Extract(T info)
        {
            if (info is null)
            {
                Debug.LogError("[UtilityEntityHolder:Extract] Tried to extract an entity from a bag but the info given was null");
                return null;
            }

            if (!Contains(info))
            {
                Debug.LogError("[UtilityEntityHolder:Extract] Tried to extract an entity from a bag but the info given wasn't in the bag");
                return null;
            }

            if (info.Decrement() < 1)
                RemoveAndForget(info);

            return info.ID;
        }

        public string Extract(string id) => Extract(Get(id));

        public T Get(string id)
        {
            if (this.utilityEntityIndex.Count < 1)
                foreach (T info in this)
                    this.utilityEntityIndex[info.ID] = info;

            if (this.utilityEntityIndex.ContainsKey(id))
                return this.utilityEntityIndex[id];

            return null;
        }

        protected T AddIfNone(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (Get(id) is not null)
                return Get(id);

            T newInfo = new()
            {
                ID = id
            };
            Add(newInfo);
            this.utilityEntityIndex[newInfo.ID] = newInfo;
            return newInfo;
        }

        protected void RemoveAndForget(T info)
        {
            Remove(info);
            this.utilityEntityIndex[info.ID] = null;
        }
    }
}