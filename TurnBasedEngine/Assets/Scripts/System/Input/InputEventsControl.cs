using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D
{
    public class InputEventsControl : InputEvents
    {
        protected virtual void Update()
        {
            if (InputCtx.One.ConfirmPress && this.confirmEnabled)
                this.confirmEvent.Invoke();

            if (InputCtx.One.MenuPress && this.menuEnabled)
                this.menuEvent.Invoke();

            if (InputCtx.One.SpecialPress && this.specialEnabled)
                this.specialEvent.Invoke();

            if (InputCtx.One.BackPress && this.backEnabled)
                this.backEvent.Invoke();

            if (InputCtx.One.PausePress && this.pauseEnabled)
                this.pauseEvent.Invoke();

            if (InputCtx.One.SelectPress && this.selectEnabled)
                this.selectEvent.Invoke();
        }
    }
}
