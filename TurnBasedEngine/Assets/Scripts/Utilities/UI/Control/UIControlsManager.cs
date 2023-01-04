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
            if (UIControlsManager.instance is not null)
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
        private readonly Mutex historyMutex = new Mutex();

        public void PassControlBack()
        {
            this.historyMutex.WaitOne();

            if (this.currentControl is not null)
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

            //Dont give control to the component that is already in control
            if (Object.ReferenceEquals(this.currentControl, uiControl))
                return;

            if (this.currentControl is not null)
            {
                this.controlChainHistory.Push(this.currentControl);
                EndControl(this.currentControl);
            }

            StartControl(uiControl);

            this.historyMutex.ReleaseMutex();
        }

        private void StartControl(UIControl uiControl)
        {
            uiControl.gameObject.SetActive(true);
            uiControl.enabled = true;
            this.currentControl = uiControl;
            uiControl.ControlInitialize();
        }

        private static void EndControl(UIControl uiControl)
        {
            uiControl.enabled = false;
            uiControl.ControlFinalize();
        }
    }
}
