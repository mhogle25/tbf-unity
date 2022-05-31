using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using BF2D.Game;

namespace BF2D.Combat
{
    public class GameInfo : MonoBehaviour
    {
        //Singleton Reference
        public static GameInfo Instance { get { return GameInfo.instance; } }
        private static GameInfo instance;

        //All characters
        private Dictionary<string, PlayerStats> players = new Dictionary<string, PlayerStats>();

        //AssetCollections
        [SerializeField] private SpriteCollection iconCollection = null;
        [SerializeField] private AudioClipCollection soundEffectCollection = null;

        private void Awake()
        {
            //Set this object not to destroy on loading new scenes
            DontDestroyOnLoad(this.gameObject);

            //Setup of Monobehaviour Singleton
            if (GameInfo.instance != this && GameInfo.instance != null)
            {
                Destroy(GameInfo.instance.gameObject);
            }

            GameInfo.instance = this;
        }

        public PlayerStats GetPlayer(string key)
        {
            if (!this.players.ContainsKey(key))
            {
                return null;
            }

            return this.players[key];
        }

        public Sprite GetIcon(string key)
        {
            return this.iconCollection.Get(key);
        }

        public AudioClip GetSoundEffect(string key)
        {
            return this.soundEffectCollection.Get(key);
        }
    }

}