using UnityEngine;
using System;
using System.Collections.Generic;
using BF2D;
using DataStructures.PriorityQueue;

public class CombatManager : MonoBehaviour
{
    public class ComboManager
    {
        [Serializable]
        private class KeystrokeGraph
        {
            [Serializable]
            private struct Keystroke
            {
                public InputKey inputKey;
                public int frames;
                public object prereq;
                public List<Keystroke> next;
            }

            Keystroke root;
        }

        private List<KeystrokeGraph> combos = new List<KeystrokeGraph>();


    }

    //Singleton Reference
    public static CombatManager Instance { get { return instance; } }
    private static CombatManager instance = null;

    private void Awake() {
        //Setup of Monobehaviour Singleton
        if (CombatManager.instance != this && CombatManager.instance != null) {
            Destroy(CombatManager.instance.gameObject);
        }
        CombatManager.instance = this;
    }

    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (CombatManager.instance != null)
        {
            Destroy(CombatManager.instance.gameObject);
        }
    }

    private void InitializeCombat()
    {

    }
}
