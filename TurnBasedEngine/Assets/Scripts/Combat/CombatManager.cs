using UnityEngine;
using System;
using System.Collections.Generic;
using BF2D;
namespace BF2D.TurnBased
{
    public class CombatManager : MonoBehaviour
    {
        /// <summary>
        /// Static reference to the last instantiated CombatManager
        /// </summary>
        public static CombatManager LastInstance { get { return lastInstance; } }
        private static CombatManager lastInstance = null;

        [SerializeField] private ComboManager comboManager = null;

        private void Awake()
        {
            CombatManager.lastInstance = this;
        }

        private void Update()
        {

        }

        private void OnDestroy()
        {
            if (CombatManager.LastInstance != null)
            {
                Destroy(CombatManager.LastInstance.gameObject);
            }
        }

        private void InitializeCombat()
        {

        }
    }
}
