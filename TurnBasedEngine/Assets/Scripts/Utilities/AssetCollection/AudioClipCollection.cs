using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D
{
    public class AudioClipCollection : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

        private readonly Dictionary<string, AudioClip> audioClipsDict = new Dictionary<string, AudioClip>();

        public AudioClip this[string key] { get { return Get(key); } }

        private void Awake()
        {
            Setup(this.audioClips);
        }

        public void Setup(List<AudioClip> clips)
        {
            foreach (AudioClip clip in clips)
            {
                this.audioClipsDict.Add(clip.name, clip);
            }
        }

        public void Clear()
        {
            this.audioClipsDict.Clear();
        }

        public AudioClip Get(string key)
        {
            if (!this.audioClipsDict.ContainsKey(key))
            {
                throw new ArgumentException($"[SpriteCollection] The sprite collection did not contain a sprite for key {key}");
            }

            return this.audioClipsDict[key];
        }

        public bool Contains(string key)
        {
            return this.audioClipsDict.ContainsKey(key);
        }

    }
}
