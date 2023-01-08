using UnityEngine;
using BF2D.Game;
using BF2D.Combat.Actions;
using BF2D.UI;
using System.Collections.Generic;

namespace BF2D.Combat
{
    public class CharacterCombat : MonoBehaviour
    {
        public CombatAction CurrentCombatAction { get { return this.currentCombatAction; } }
        private CombatAction currentCombatAction = null;

        public CombatGridTile Tile { set { this.assignedTile = value; } }
        private CombatGridTile assignedTile = null;
        public CharacterStats Stats
        {
            get
            {
                return this.stats;
            }
            set
            {
                this.stats = value;
            }
        }
        private CharacterStats stats;

        #region Public Utilities
        public void SetupCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
            this.currentCombatAction.Setup();
        }

        public void Destroy()
        {
            if (this.assignedTile)
                this.assignedTile.ResetTile();

            Destroy(this.gameObject);
        }

        public void PlayOpeningMessage(DialogTextbox textbox)
        {
            OrphanedTextboxDialog(textbox, this.currentCombatAction.CurrentInfo.GetOpeningMessage());
        }

        public void PerformActionAnimation()
        {

        }
        #endregion

        #region Private Methods
        private void OrphanedTextboxDialog(DialogTextbox textbox, List<string> dialog)
        {
            textbox.Dialog(dialog, 0, () =>
            {
                textbox.UtilityFinalize();
            }, new List<string>
            {
               this.stats.Name
            });
            textbox.View.gameObject.SetActive(true);
            textbox.UtilityInitialize();
        }
        #endregion
    }
}
