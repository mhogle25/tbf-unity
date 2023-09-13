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

        public void TakeControl()
        {
            UICtx.One.TakeControl(this, this.threadID);
        }

        /// <summary>
        /// Pushes a single message to the dialog queue
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        public void Message(string message)
        {
            this.dialogTextbox.Message(message);
        }

        /// <summary>
        /// Pushes a single message to the dialog queue with a callback function
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <param name="callback">Called at the end of dialog</param>
        public void Message(string message, Action callback)
        {
            this.dialogTextbox.Message(message, callback);
        }

        /// <summary>
        /// Pushes a single message to the dialog queue with a callback function
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <param name="callback">Called at the end of dialog</param>
        public void Message(string message, Action callback, params string[] inserts)
        {
            this.dialogTextbox.Message(message, callback, inserts);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue
        /// </summary>
        /// <param name="key">The filename of the desired dialog</param>
        /// <param name="startingLine">The line the dialog will start from (0 is the first line)</param>
        public void Dialog(string key, int startingLine)
        {
            this.dialogTextbox.Dialog(key, startingLine);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue with a callback function
        /// </summary>
        /// <param name="key">The filename of the desired dialog</param>
        /// <param name="startingLine">The line the dialog will start from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        public void Dialog(string key, int startingLine, Action callback)
        {
            this.dialogTextbox.Dialog(key, startingLine, callback);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue with a callback function
        /// </summary>
        /// <param name="key">The filename of the desired dialog</param>
        /// <param name="startingLine">The line the dialog will start from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        public void Dialog(string key, int startingLine, Action callback, params string[] inserts)
        {
            this.dialogTextbox.Dialog(key, startingLine, callback, inserts);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="startingLine">The line the dialog starts from (0 is the first line)</param>
        public void Dialog(List<string> lines, int startingLine)
        {
            this.dialogTextbox.Dialog(lines, startingLine);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="startingLine">The line the dialog starts from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        public void Dialog(List<string> lines, int startingLine, Action callback)
        {
            this.dialogTextbox.Dialog(lines, startingLine, callback);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="startingLine">The line the dialog starts from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        public void Dialog(List<string> lines, int startingLine, Action callback, params string[] inserts)
        {
            this.dialogTextbox.Dialog(lines, startingLine, callback, inserts);
        }

        public void AddResponseController(IResponseController controller) => this.dialogTextbox.AddResponseController(controller);

        public void RemoveResponseController(IResponseController controller) => this.dialogTextbox.RemoveResponseController(controller);

        public void ClearResponseControllers() => this.dialogTextbox.ClearResponseControllers();

        public void SetViewActive(bool active) => this.dialogTextbox.View.gameObject.SetActive(active);
    }
}
