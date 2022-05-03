using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public abstract class UIUtility : MonoBehaviour
{
    public virtual bool Interactable { get { return this.interactable; } set { this.interactable = value; } }
    [SerializeField] private protected bool interactable = true;

    public virtual RectTransform View { get { return this.view; } }
    [SerializeField] private protected RectTransform view;

    private protected static Stack<UIUtility> controlChainHistory = new Stack<UIUtility>();

    public void PassControl(UIUtility successor)
    {
        UIUtility.controlChainHistory.Push(this);
        successor.StartControlChain();
        this.Interactable = false;
    }

    public void PassControlBack()
    {
        if (UIUtility.controlChainHistory.Count > 0)
        {
            UIUtility ancestor = UIUtility.controlChainHistory.Pop();
            ancestor.StartControlChain();
            this.Interactable = false;
        }
    }

    public void PassControlBackToFirst(bool setActive)
    {
        UIUtility.controlChainHistory.Push(this);

        while (UIUtility.controlChainHistory.Count > 0)
        {
            UIUtility uiUtility = UIUtility.controlChainHistory.Pop();

            if (UIUtility.controlChainHistory.Count > 1)
            {
                uiUtility.Interactable = false;
                uiUtility.View.gameObject.SetActive(setActive);
            } 
            else
            {
                uiUtility.StartControlChain();
            }
        }
    }

    public void ResetControlChain(bool setActive)
    {
        UIUtility.controlChainHistory.Push(this);

        while (UIUtility.controlChainHistory.Count > 0)
        {
            UIUtility uiUtility = UIUtility.controlChainHistory.Pop();
            uiUtility.Interactable = false;
            uiUtility.View.gameObject.SetActive(setActive);
        }
    }

    public void ClearControlChainHistory()
    {
        UIUtility.controlChainHistory.Clear();
    }

    public void StartControlChain()
    {
        this.View.gameObject.SetActive(true);
        this.Interactable = true;
    }

    private protected void PlayAudioSource(AudioSource audioSource)
    {
        if (audioSource == null)
            return;

        if (audioSource.clip != null)
            audioSource.Play();
    }
    private void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        ClearControlChainHistory();
    }

    private void OnDestroy()
    {
        if (UIUtility.controlChainHistory.Contains(this))
        {
            Stack<UIUtility> temp = new Stack<UIUtility>();
            while (UIUtility.controlChainHistory.Count > 0)
            {
                if (UIUtility.controlChainHistory.Peek() != this)
                {
                    temp.Push(UIUtility.controlChainHistory.Pop());
                }
            }

            while (temp.Count > 0)
            {
                UIUtility.controlChainHistory.Push(temp.Pop());
            }
        }
    }
}
