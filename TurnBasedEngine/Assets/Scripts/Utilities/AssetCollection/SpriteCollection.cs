using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D
{
    public class SpriteCollection : MonoBehaviour
    {
        [SerializeField] private List<Sprite> sprites = new();

        private readonly Dictionary<string, Sprite> spritesDict = new();

        public Sprite this[string id] { 
            get 
            { 
                return Get(id); 
            }

            set
            {
                this.spritesDict[id] = value;
            }
        }

        private void Awake()
        {
            Setup(this.sprites);
        }

        public void Setup(List<Sprite> sprites)
        {
            foreach (Sprite s in sprites)
            {
                this.spritesDict.Add(s.name, s);
            }
        }

        public void Clear()
        {
            this.spritesDict.Clear();
        }

        public Sprite Get(string id)
        {
            if (!this.spritesDict.ContainsKey(id))
            {
                Debug.LogError($"[SpriteCollection:Get] The sprite collection did not contain a sprite for id {id}");
                return null;
            }

            return this.spritesDict[id];
        }

        public bool Contains(string id)
        {
            return this.spritesDict.ContainsKey(id);
        }
    }
}
