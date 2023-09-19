using UnityEngine;
using System;
using System.Collections.Generic;
using BF2D.Game;

namespace BF2D.UI 
{ 
    public class DialogTextboxControl : UIControl
    {
        [SerializeField] private DialogTextbox dialogTextbox = null;
        [SerializeField] private string threadID = Strings.UI.THREAD_MAIN;

        protected virtual void Update()
        {
            if (InputCtx.One.ConfirmPress)
                this.dialogTextbox.Continue();
        }

        public override void ControlInitialize()
        {
            this.dialogTextbox.UtilityInitialize();
        }

        public override void ControlFinalize()
        {
            this.dialogTextbox.UtilityFinalize();
        }

        public bool Armed => this.dialogTextbox.Armed;

        public void TakeControl() =>
            UICtx.One.TakeControl(this, this.threadID);

        /// <summary>
        /// Pushes a single message to the dialog queue with a callback function
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <param name="callback">Called at the end of dialog</param>
        /// <param name="inserts">Will replace insert tags in dialog by index</param>
        public void Message(string message = null, Action callback = null, params string[] inserts)
        {
            this.dialogTextbox.Message(message, callback, inserts);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue with a callback function
        /// </summary>
        /// <param name="id">The filename of the desired dialog</param>
        /// <param name="startingLine">The line the dialog will start from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        /// <param name="inserts">Will replace insert tags in dialog by index</param>
        public void Dialog(string id = null, int startingLine = 0, Action callback = null, params string[] inserts)
        {
            this.dialogTextbox.Dialog(id, startingLine, callback, inserts);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="startingLine">The line the dialog starts from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        /// <param name="inserts">Will replace insert tags in dialog by index</param>
        public void Dialog(List<string> lines = null, int startingLine = 0, Action callback = null, params string[] inserts)
        {
            this.dialogTextbox.Dialog(lines, startingLine, callback, inserts);
        }

        public void AddResponseController(IResponseController controller) => this.dialogTextbox.AddResponseController(controller);

        public void RemoveResponseController(IResponseController controller) => this.dialogTextbox.RemoveResponseController(controller);

        public void ClearResponseControllers() => this.dialogTextbox.ClearResponseControllers();

        public void SetViewActive(bool active) => this.dialogTextbox.View.gameObject.SetActive(active);
    }
}
