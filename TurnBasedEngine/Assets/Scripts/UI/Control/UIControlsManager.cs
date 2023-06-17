using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace BF2D.UI
{
    public class UIControlsManager : MonoBehaviour
    {
        public InputEventsControl SystemInputEvents => this.inputEventsControl;
        [SerializeField] private InputEventsControl inputEventsControl = null;

        public static UIControlsManager Instance => UIControlsManager.instance;
        private static UIControlsManager instance = null;

        private readonly Dictionary<string, Stack<UIControl>> controlHistory = new() { { Game.Strings.UI.MainThread, new() } };
        private readonly Stack<string> threadHistory = new();
        private UIControl currentControl = null;

        private string currentThreadID = Game.Strings.UI.MainThread;

        private void Awake()
        {
            //Setup of Monobehaviour Singleton
            if (UIControlsManager.instance && UIControlsManager.instance != this)
                Destroy(UIControlsManager.instance.gameObject);

            UIControlsManager.instance = this;
        }

        public void PassControlBack()
        {
            if (this.currentControl)
                EndControl(this.currentControl);

            if (this.threadHistory.Count > 0 && this.controlHistory[this.currentThreadID].Count < 1)
                this.currentThreadID = this.threadHistory.Pop();

            Stack<UIControl> stack = this.controlHistory[this.currentThreadID];

            if (stack.Count > 0)
                StartControl(stack.Pop());
            else
                this.currentControl = null;
        }

        public void PassControlBackToFirst(bool setActive)
        {
            Stack<UIControl> stack = this.controlHistory[this.currentThreadID];

            stack.Push(this.currentControl);
            this.currentControl = null;

            while (stack.Count > 0)
            {
                UIControl uiControl = stack.Pop();

                if (stack.Count > 1)
                {
                    EndControl(uiControl);
                    uiControl.gameObject.SetActive(setActive);
                }
                else
                {
                    StartControl(uiControl);
                }
            }
        }

        public void PassControlBackToFirst(bool setActive, string threadID)
        {
            if (!this.controlHistory.ContainsKey(threadID))
            {
                Debug.LogError($"[UIControlsManager:PassControlBackToFirst] Invalid thread ID '{threadID}'");
                return;
            }

            ChangeThread(threadID);
            PassControlBackToFirst(setActive);
        }

        public void ResetControlChain(bool setActive)
        { 
            Stack<UIControl> stack = this.controlHistory[this.currentThreadID];

            if (stack.Count < 1 && this.currentControl == null)
                return;

            stack.Push(this.currentControl);
            this.currentControl = null;

            while (stack.Count > 0)
            {
                UIControl uiControl = stack.Pop();
                EndControl(uiControl);
                uiControl.gameObject.SetActive(setActive);
            }
        }

        public void ResetControlChain(bool setActive, string threadID)
        {
            if (!this.controlHistory.ContainsKey(threadID))
            {
                Debug.LogError($"[UIControlsManager:ResetControlChain] Invalid thread ID '{threadID}'");
                return;
            }

            ChangeThread(threadID);
            ResetControlChain(setActive);
        }

        public void ClearControlChainHistory()
        {
            this.controlHistory[this.currentThreadID].Clear();
        }

        public void ClearControlChainHistory(string threadID)
        {
            if (!this.controlHistory.ContainsKey(threadID))
            {
                Debug.LogError($"[UIControlsManager:ClearControlChainHistory] Invalid thread ID '{threadID}'");
                return;
            }

            ChangeThread(threadID);
            ClearControlChainHistory();
        }

        public void TakeControl(UIControl uiControl)
        {
            //Dont give control to the component that is already in control
            if (Object.ReferenceEquals(this.currentControl, uiControl))
                return;

            if (this.currentControl)
            {
                this.controlHistory[this.currentThreadID].Push(this.currentControl);
                EndControl(this.currentControl);
            }

            StartControl(uiControl);
        }

        public void TakeControl(UIControl uiControl, string threadID)
        {
            if (!this.controlHistory.ContainsKey(threadID))
            {
                Debug.LogError($"[UIControlsManager:TakeControl] Invalid thread ID '{threadID}'");
                return;
            }

            TakeControl(uiControl);
            ChangeThread(threadID);
        }

        private void StartControl(UIControl uiControl)
        {
            this.currentControl = uiControl;
            StartControlGeneric(uiControl);
        }

        private void EndControl(UIControl uiControl)
        {
            EndControlGeneric(uiControl);
        }

        private void ChangeThread(string threadID)
        {
            if (!this.controlHistory.ContainsKey(threadID))
                this.controlHistory[threadID] = new();

            if (this.threadHistory.Count > 0 && this.threadHistory.Peek() != this.currentThreadID)
                this.threadHistory.Push(this.currentThreadID);

            this.currentThreadID = threadID;
        }

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
    }
}
