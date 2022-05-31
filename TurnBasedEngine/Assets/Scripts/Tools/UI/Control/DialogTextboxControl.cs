using UnityEngine;
using System;

namespace BF2D.UI 
{ 
    public class DialogTextboxControl : UIControl
    {
        public DialogTextbox Textbox { get { return this.dialogTextbox; } }
        [SerializeField] private DialogTextbox dialogTextbox = null;

        private Action state = null;

        protected virtual void Awake()
        {
            this.state = Listen;
        }

        protected virtual void Update()
        {
            if (this.Interactable)
                this.state?.Invoke();
        }

        private void Listen()
        {
            if (InputManager.ConfirmPress)
            {
                this.dialogTextbox.Continue();
            }
        }

        public override void ControlInitialize()
        {
            this.dialogTextbox.UtilityInitialize();
        }

        public override void ControlFinalize()
        {
            this.dialogTextbox.UtilityFinalize();
        }
    }
}
