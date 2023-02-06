using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace BF2D.UI
{
    public class UIControlsManager : MonoBehaviour
    {
        public InputEventsControl EventsControl { get { return this.inputEventsControl; } }
        [SerializeField] private InputEventsControl inputEventsControl = null;

        public static UIControlsManager Instance { get { return UIControlsManager.instance; } }
        private static UIControlsManager instance = null;

        private void Awake()
        {
            //Setup of Monobehaviour Singleton
            if (UIControlsManager.instance)
            {
                if (UIControlsManager.instance != this)
                {
                    Destroy(UIControlsManager.instance.gameObject);
                }
            }

            UIControlsManager.instance = this;
        }

        private readonly Stack<UIControl> controlChainHistory = new Stack<UIControl>();
        private UIControl currentControl = null;
        private UIControl phantomControl = null;
        private readonly Mutex historyMutex = new();

        public void PassControlBack()
        {
            this.historyMutex.WaitOne();

            if (this.phantomControl)
            {
                Debug.LogWarning("[UIControlsManager:PassControlBack] Cannot perform UIControl History operations while a phantom UIControl is active.");
                return;
            }

            if (this.currentControl)
                EndControl(this.currentControl);

            if (this.controlChainHistory.Count > 0)
            {
                UIControl ancestor = this.controlChainHistory.Pop();
                StartControl(ancestor);
            } 
            else
            {
                this.currentControl = null;
            }

            this.historyMutex.ReleaseMutex();
        }

        public void PassControlBackToFirst(bool setActive)
        {
            this.historyMutex.WaitOne();

            if (this.phantomControl)
            {
                Debug.LogWarning("[UIControlsManager:PassControlBackToFirst] Cannot perform UIControl History operations while a phantom UIControl is active.");
                return;
            }

            this.controlChainHistory.Push(this.currentControl);
            this.currentControl = null;

            while (this.controlChainHistory.Count > 0)
            {
                UIControl uiControl = this.controlChainHistory.Pop();

                if (this.controlChainHistory.Count > 1)
                {
                    EndControl(uiControl);
                    uiControl.gameObject.SetActive(setActive);
                }
                else
                {
                    StartControl(uiControl);
                }
            }

            this.historyMutex.ReleaseMutex();
        }

        public void ResetControlChain(bool setActive)
        {
            this.historyMutex.WaitOne();

            if (this.phantomControl)
            {
                Debug.LogWarning("[UIControlsManager:ResetControlChain] Cannot perform UIControl History operations while a phantom UIControl is active.");
                return;
            }

            this.controlChainHistory.Push(this.currentControl);
            this.currentControl = null;

            while (this.controlChainHistory.Count > 0)
            {
                UIControl uiControl = this.controlChainHistory.Pop();
                EndControl(uiControl);
                uiControl.gameObject.SetActive(setActive);
            }

            this.historyMutex.ReleaseMutex();
        }

        public void ClearControlChainHistory()
        {
            this.historyMutex.WaitOne();

            this.controlChainHistory.Clear();

            this.historyMutex.ReleaseMutex();
        }

        public void TakeControl(UIControl uiControl)
        {
            this.historyMutex.WaitOne();

            if (this.phantomControl)
            {
                Debug.LogWarning("[UIControlsManager:TakeControl] Cannot perform UIControl History operations while a phantom UIControl is active.");
                return;
            }

            //Dont give control to the component that is already in control
            if (Object.ReferenceEquals(this.currentControl, uiControl))
                return;

            if (this.currentControl)
            {
                this.controlChainHistory.Push(this.currentControl);
                EndControl(this.currentControl);
            }

            StartControl(uiControl);

            this.historyMutex.ReleaseMutex();
        }

        public void StartPhantomControl(UIControl uiControl)
        {
            if (this.phantomControl)
                EndControlGeneric(this.phantomControl);

            this.phantomControl = uiControl;
            StartControlGeneric(this.phantomControl);
            if (this.currentControl)
                this.currentControl.enabled = false;
        }

        public void EndPhantomControl()
        {
            if (!this.phantomControl)
            {
                Debug.LogWarning("[UIControlsManager:EndPhantomControl] Tried to end a phantom UIControl but none were active.");
                return;
            }

            EndControlGeneric(this.phantomControl);
            this.phantomControl = null;
            if (this.currentControl)
                this.currentControl.enabled = true;
        }

        private void StartControl(UIControl uiControl)
        {
            StartControlGeneric(uiControl);
            this.currentControl = uiControl;
        }

        private void EndControl(UIControl uiControl)
        {
            EndControlGeneric(uiControl);
        }

        private void StartControlGeneric(UIControl uiControl)
        {
            uiControl.gameObject.SetActive(true);
            uiControl.enabled = true;
            uiControl.ControlInitialize();
        }

        private static void EndControlGeneric(UIControl uiControl)
        {
            uiControl.enabled = false;
            uiControl.ControlFinalize();
        }
    }
}
