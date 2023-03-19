using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class InputEventsControl : InputEvents
    {
        private void Update()
        {
            if (InputManager.Instance.ConfirmPress && this.confirmEnabled)
            {
                this.confirmEvent.Invoke();
            }

            if (InputManager.Instance.MenuPress && this.menuEnabled)
            {
                this.menuEvent.Invoke();
            }

            if (InputManager.Instance.SpecialPress && this.specialEnabled)
            {
                this.specialEvent.Invoke();
            }

            if (InputManager.Instance.BackPress && this.backEnabled)
            {
                this.backEvent.Invoke();
            }

            if (InputManager.Instance.PausePress && this.pauseEnabled)
            {
                this.pauseEvent.Invoke();
            }

            if (InputManager.Instance.SelectPress && this.selectEnabled)
            {
                this.selectEvent.Invoke();
            }
        }
    }
}
