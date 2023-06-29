using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class AudioClipCollection : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> audioClips = new();

        private readonly Dictionary<string, AudioClip> audioClipsDict = new();

        public AudioClip this[string id] => Get(id);

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

        public AudioClip Get(string id)
        {
            if (!this.audioClipsDict.ContainsKey(id))
            {
                Debug.LogError($"[SpriteCollection] The audio collection did not contain a sound for id {id}");
                return null;
            }

            return this.audioClipsDict[id];
        }

        public bool Contains(string id)
        {
            return this.audioClipsDict.ContainsKey(id);
        }

    }
}
