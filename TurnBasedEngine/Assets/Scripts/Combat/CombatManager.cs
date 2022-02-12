using UnityEngine;
using System;
using BF2D;

public class CombatManager : MonoBehaviour
{
    //Singleton Reference
    public static CombatManager Instance { get { return instance; } }
    private static CombatManager instance;

    private void Awake() {
        //Setup of Monobehaviour Singleton
        if (CombatManager.instance != this && CombatManager.instance != null) {
            Destroy(CombatManager.instance.gameObject);
        }
        CombatManager.instance = this;
    }
    
    
}
