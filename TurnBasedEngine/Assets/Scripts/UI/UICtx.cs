using System.Collections.Generic;
using UnityEngine;

namespace BF2D.UI
{
    public class UICtx : MonoBehaviourSingleton<UICtx>
    {
        public InputEventsControl SystemInputEvents => this.inputEventsControl;
        public DialogTextboxControl SystemTextbox => this.systemTextbox;

        [Header("Utilities")]
        [SerializeField] private InputEventsControl inputEventsControl = null;
        [SerializeField] private DialogTextboxControl systemTextbox = null;
        [Header("Info")]
        [SerializeField] private string currentThreadID = Game.Strings.UI.THREAD_MAIN;
        [SerializeField] private UIControl currentControl = null;

        private readonly Dictionary<string, Stack<UIControl>> controlHistory = new() { { Game.Strings.UI.THREAD_MAIN, new() } };
        private readonly Stack<string> threadHistory = new();

        private Stack<UIControl> CurrentStack => this.controlHistory[this.currentThreadID];

        public bool IsControlling(UIControl control) => this.currentControl != null && this.currentControl.Equals(control) && this.currentControl.enabled;
        public UIControl Current => this.currentControl;

        public static void StartControlGeneric(UIControl uiControl)
        {
            uiControl.gameObject.SetActive(true);
            uiControl.enabled = true;
            uiControl.ControlInitialize();
        }

        public static void EndControlGeneric(UIControl uiControl)
        {
            uiControl.enabled = false;
            uiControl.ControlFinalize();
        }

        public void PassControlBack()
        {
            if (this.currentControl)
                EndControl(this.currentControl);

            if (this.threadHistory.Count > 0 && this.CurrentStack.Count < 1)
            {
                this.currentThreadID = this.threadHistory.Pop();

                // Debug.Log($"Popped thread from history: {this.currentThreadID}");
            }

            Stack<UIControl> stack = this.CurrentStack;

            if (stack.Count > 0)
            {
                UIControl uiControl = stack.Pop();

                //Debug.Log($"Pass Control back to: {uiControl.name} from: {this.currentControl.name}");

                StartControl(uiControl);
            }
            else
            {
                // Debug.Log($"Pass Control back from: {this.currentControl.name}");

                this.currentControl = null;
            }
        }

        public void PassControlBackToFirst(bool setActive)
        {
            Stack<UIControl> stack = this.CurrentStack;

            stack.Push(this.currentControl);
            this.currentControl = null;

            while (stack.Count > 0)
            {
                UIControl uiControl = stack.Pop();

                if (stack.Count > 1)
                {
                    EndControl(uiControl);

                    // Debug.Log($"Pass control back from: {uiControl.name}");

                    uiControl.gameObject.SetActive(setActive);
                }
                else
                {
                    // Debug.Log($"Pass Control back to: {uiControl.name}");

                    StartControl(uiControl);
                }
            }
        }

        public void PassControlBackToFirst(bool setActive, string threadID)
        {
            ChangeThread(threadID);
            PassControlBackToFirst(setActive);
        }

        public void ResetControlChain(bool setActive)
        {
            Stack<UIControl> stack = this.CurrentStack;

            if (stack.Count < 1 && this.currentControl == null)
                return;

            stack.Push(this.currentControl);
            this.currentControl = null;

            while (stack.Count > 0)
            {
                UIControl uiControl = stack.Pop();

                // Debug.Log($"Pass control back from: {uiControl.name}");

                EndControl(uiControl);
                uiControl.gameObject.SetActive(setActive);
            }
        }

        public void ResetControlChain(bool setActive, string threadID)
        {
            ChangeThread(threadID);
            ResetControlChain(setActive);
        }

        public void ClearControlChainHistory()
        {
            this.CurrentStack.Clear();
        }

        public void ClearControlChainHistory(string threadID)
        {
            ChangeThread(threadID);
            ClearControlChainHistory();
        }

        public void TakeControl(UIControl uiControl)
        {
            // Debug.Log($"Take Control: {uiControl.name}");

            //Dont give control to the component that is already in control
            if (Object.ReferenceEquals(this.currentControl, uiControl))
                return;

            if (this.currentControl)
            {
                this.CurrentStack.Push(this.currentControl);
                EndControl(this.currentControl);
            }

            StartControl(uiControl);
        }

        public void TakeControl(UIControl uiControl, string threadID)
        {
            TakeControl(uiControl);
            ChangeThread(threadID);
        }

        private void StartControl(UIControl uiControl)
        {
            this.currentControl = uiControl;
            UICtx.StartControlGeneric(uiControl);
        }

        private void EndControl(UIControl uiControl)
        {
            UICtx.EndControlGeneric(uiControl);
        }

        private void ChangeThread(string threadID)
        {
            if (!this.controlHistory.ContainsKey(threadID))
                this.controlHistory[threadID] = new();

            string topOfThreadHistory = this.currentThreadID;

            // Debug.Log($"New Thread: {threadID} Top Of Thread History: {topOfThreadHistory}");

            if (threadID != topOfThreadHistory)
            {
                this.threadHistory.Push(topOfThreadHistory);
                //Debug.Log($"Pushed thread: {topOfThreadHistory}");
            }

            this.currentThreadID = threadID;

            // Debug.Log($"Changed thread to: {threadID}");
        }
    }
}