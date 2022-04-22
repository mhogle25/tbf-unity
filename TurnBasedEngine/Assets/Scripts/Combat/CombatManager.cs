using UnityEngine;
using System;
using System.Collections.Generic;
using BF2D;

namespace BF2D.TurnBased
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance { get { return instance; } }
        private static CombatManager instance = null;

        [SerializeField] private ComboManager comboManager = null;
        private Action state;

        private void Awake()
        {
            //Setup of Monobehaviour Singleton
            if (CombatManager.instance != this && CombatManager.instance != null)
            {
                Destroy(CombatManager.instance.gameObject);
            }
            CombatManager.instance = this;
        }

        private void Update()
        {
            if (state != null)
            {
                state();
            }
        }

        public void InitializeCombat()
        {

        }
    }
}
