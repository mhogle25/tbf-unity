using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public abstract class AssetCollection<T> : MonoBehaviour, ICache where T : Object
    {
        [SerializeField] private List<T> assets = new();

        private readonly Dictionary<string, T> assetsDict = new();

        public T this[string id] => Get(id);

        private void Awake()
        {
            Setup(this.assets);
        }

        public void Setup(List<T> assets)
        {
            Clear();
            foreach (T a in assets)
                this.assetsDict.Add(a.name, a);
        }

        public void Clear()
        {
            this.assetsDict.Clear();
        }

        public T Get(string id)
        {
            if (assetsDict.Count < 1)
                Setup(this.assets);

            if (!this.assetsDict.ContainsKey(id))
            {
                Debug.LogError($"[AssetCollection:Get] The collection did not contain an asset for id {id}");
                return null;
            }

            return this.assetsDict[id];
        }

        public bool Contains(string id)
        {
            return this.assetsDict.ContainsKey(id);
        }
    }
}
