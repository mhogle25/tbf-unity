using UnityEngine;
using System;
using System.Collections.Generic;
using BF2D.Game;

namespace BF2D.UI 
{ 
    public class DialogTextboxControl : UIControl
    {
        [SerializeField] private DialogTextbox dialogTextbox = null;
        [SerializeField] private string threadID = Strings.UI.MainThread;

        protected virtual void Update()
        {
            if (InputManager.Instance.ConfirmPress)
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
            UIControlsManager.Instance.TakeControl(this, this.threadID);
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
        public void Message(string message, Action callback, string[] inserts)
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
        public void Dialog(string key, int startingLine, Action callback, string[] inserts)
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
        public void Dialog(List<string> lines, int startingLine, Action callback, string[] inserts)
        {
            this.dialogTextbox.Dialog(lines, startingLine, callback, inserts);
        }
    }
}
