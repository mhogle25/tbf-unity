using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D
{
    public class SpriteCollection : MonoBehaviour
    {
        [SerializeField] private List<Sprite> sprites = new List<Sprite>();

        private readonly Dictionary<string, Sprite> spritesDict = new Dictionary<string, Sprite>();

        public Sprite this[string key] { 
            get 
            { 
                return Get(key); 
            }

            set
            {
                this.spritesDict[key] = value;
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

        public Sprite Get(string key)
        {
            if (!this.spritesDict.ContainsKey(key))
            {
                throw new ArgumentException($"[SpriteCollection] The sprite collection did not contain a sprite for key {key}");
            }

            return this.spritesDict[key];
        }

        public bool Contains(string key)
        {
            return this.spritesDict.ContainsKey(key);
        }
    }
}
