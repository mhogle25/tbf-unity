using UnityEngine;
using System;
using System.Collections.Generic;

namespace BF2D.Combat
{
    public class CombatManager : MonoBehaviour
    {
        public ComboManager ComboManagerComponent { get { return comboManager; } }
        [SerializeField] private ComboManager comboManager = null;
        public StatsPreviewController StatsPreviewControllerComponent { get { return this.statsPreviewController; } }
        [SerializeField] private StatsPreviewController statsPreviewController = null;

        /*
        public CombatState State { 
            set 
            {
                this.state?.End();
                this.state = value;
                this.state.Start();
            } 
        }

        private CombatState state = null;
        private CSPlayerSelection statePlayerSelection = null;

        public CombatManager()
        {
            this.statePlayerSelection = new CSPlayerSelection(this);
        }
        */

        private void Update()
        {
            //this.state?.Update();
        }

        private void InitializeCombat()
        {
            //this.state = this.statePlayerSelection;
        }
    }
}
