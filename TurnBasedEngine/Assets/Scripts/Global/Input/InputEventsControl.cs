using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class InputEventsControl : InputEvents
    {
        private void Update()
        {
            if (InputManager.ConfirmPress && this.confirmEnabled)
            {
                this.confirmEvent.Invoke();
            }

            if (InputManager.MenuPress && this.menuEnabled)
            {
                this.menuEvent.Invoke();
            }

            if (InputManager.AttackPress && this.attackEnabled)
            {
                this.attackEvent.Invoke();
            }

            if (InputManager.BackPress && this.backEnabled)
            {
                this.backEvent.Invoke();
            }

            if (InputManager.PausePress && this.pauseEnabled)
            {
                this.pauseEvent.Invoke();
            }

            if (InputManager.SelectPress && this.selectEnabled)
            {
                this.selectEvent.Invoke();
            }
        }
    }
}
