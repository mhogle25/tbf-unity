using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace BF2D.UI
{
    public class UIControlsManager : MonoBehaviour
    {
        [SerializeField] private UIControl initializeOnStart = null;

        public static UIControlsManager Instance { get { return UIControlsManager.instance; } }
        private static UIControlsManager instance = null;

        private void Awake()
        {

            //Set this object not to destroy on loading new scenes
            DontDestroyOnLoad(this.gameObject);

            //Setup of Monobehaviour Singleton
            if (UIControlsManager.instance != this && UIControlsManager.instance != null)
            {
                Destroy(UIControlsManager.instance.gameObject);
            }

            UIControlsManager.instance = this;
        }

        void Start()
        {
            if (!(this.initializeOnStart is null))
            {
                TakeControl(this.initializeOnStart);
            }
        }

        private readonly Stack<UIControl> controlChainHistory = new Stack<UIControl>();
        private UIControl currentControl = null;
        private readonly Mutex historyMutex = new Mutex();

        public void PassControlBack()
        {
            this.historyMutex.WaitOne();

            if (this.controlChainHistory.Count > 0)
            {
                UIControl ancestor = this.controlChainHistory.Pop();
                if (this.currentControl)
                    EndControl(this.currentControl);
                StartControl(ancestor);
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

            if (!(this.currentControl is null))
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
            uiControl.Interactable = true;
            uiControl.ControlInitialize();
            this.currentControl = uiControl;
        }

        private static void EndControl(UIControl uiControl)
        {
            uiControl.Interactable = false;
            uiControl.ControlFinalize();
        }
    }
}
