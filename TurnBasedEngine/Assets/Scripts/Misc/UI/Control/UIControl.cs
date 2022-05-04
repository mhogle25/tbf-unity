using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;

namespace BF2D.UI
{
    public abstract class UIControl : UIComponent
    {
        private static Stack<UIControl> controlChainHistory = new Stack<UIControl>();
        private static Mutex historyMutex = new Mutex();

        public void PassControl(UIControl successor)
        {
            UIControl.historyMutex.WaitOne();

            UIControl.controlChainHistory.Push(this);
            successor.StartControlChain();
            this.Interactable = false;
            ControlFinalize();

            UIControl.historyMutex.ReleaseMutex();
        }

        public void PassControlBack()
        {
            UIControl.historyMutex.WaitOne();

            if (UIControl.controlChainHistory.Count > 0)
            {
                UIControl ancestor = UIControl.controlChainHistory.Pop();
                ancestor.StartControlChain();
                this.Interactable = false;
                ControlFinalize();
            }

            UIControl.historyMutex.ReleaseMutex();
        }

        public void PassControlBackToFirst(bool setActive)
        {
            UIControl.historyMutex.WaitOne();

            UIControl.controlChainHistory.Push(this);

            while (UIControl.controlChainHistory.Count > 0)
            {
                UIControl UIControl = UIControl.controlChainHistory.Pop();

                if (UIControl.controlChainHistory.Count > 1)
                {
                    UIControl.Interactable = false;
                    UIControl.gameObject.SetActive(setActive);
                    UIControl.ControlFinalize();
                }
                else
                {
                    UIControl.StartControlChain();
                }
            }

            UIControl.historyMutex.ReleaseMutex();
        }

        public void ResetControlChain(bool setActive)
        {
            UIControl.historyMutex.WaitOne();

            UIControl.controlChainHistory.Push(this);
            while (UIControl.controlChainHistory.Count > 0)
            {
                UIControl UIControl = UIControl.controlChainHistory.Pop();
                UIControl.Interactable = false;
                UIControl.gameObject.SetActive(setActive);
                UIControl.ControlFinalize();
            }

            UIControl.historyMutex.ReleaseMutex();
        }

        public void ClearControlChainHistory()
        {
            UIControl.historyMutex.WaitOne();

            UIControl.controlChainHistory.Clear();

            UIControl.historyMutex.ReleaseMutex();
        }

        public void StartControlChain()
        {
            this.gameObject.SetActive(true);
            this.Interactable = true;
            ControlInitialize();
        }

        protected abstract void ControlInitialize();

        protected abstract void ControlFinalize();

        protected virtual void OnEnable()
        {
            //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        protected virtual void OnDisable()
        {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            ClearControlChainHistory();
        }

        protected virtual void OnDestroy()
        {
            UIControl.historyMutex.WaitOne();

            if (UIControl.controlChainHistory.Contains(this))
            {
                Stack<UIControl> temp = new Stack<UIControl>();
                while (UIControl.controlChainHistory.Count > 0)
                {
                    if (UIControl.controlChainHistory.Peek() != this)
                    {
                        temp.Push(UIControl.controlChainHistory.Pop());
                    }
                }

                while (temp.Count > 0)
                {
                    UIControl.controlChainHistory.Push(temp.Pop());
                }
            }

            UIControl.historyMutex.ReleaseMutex();
        }
    }

}
