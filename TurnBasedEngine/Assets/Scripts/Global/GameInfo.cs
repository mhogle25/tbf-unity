using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace BF2D.Combat
{
    public class GameInfo : MonoBehaviour
    {
        //Singleton Reference
        public static GameInfo Instance { get { return GameInfo.instance; } }
        private static GameInfo instance;
        //All characters

        private Dictionary<string, PlayerStats> players = new Dictionary<string, PlayerStats>();

        private void Awake()
        {
            //Set this object not to destroy on loading new scenes
            DontDestroyOnLoad(gameObject);

            //Setup of Monobehaviour Singleton
            if (GameInfo.instance != this && GameInfo.instance != null)
            {
                Destroy(GameInfo.instance.gameObject);
            }

            GameInfo.instance = this;
        }

        public PlayerStats GetPlayer(string key)
        {
            if (!players.ContainsKey(key))
            {
                return null;
            }

            return players[key];
        }
    }

}