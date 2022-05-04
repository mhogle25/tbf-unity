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

        protected override void ControlInitialize()
        {
            this.dialogTextbox.View.gameObject.SetActive(true);
            this.dialogTextbox.Interactable = true;
        }

        protected override void ControlFinalize()
        {
            this.dialogTextbox.Interactable = false;
        }
    }
}
