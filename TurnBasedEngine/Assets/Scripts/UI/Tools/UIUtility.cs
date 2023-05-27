using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace BF2D.UI
{
    public abstract class UIUtility : MonoBehaviour, IUIComponent
    {

        public Transform View => this.view;
        [Header("UIUtility")]
        [SerializeField] protected Transform view = null;

        public bool Interactable { get => this.interactable; set => this.interactable = value; }
        [SerializeField] protected bool interactable = false;

        public virtual void UtilityInitialize()
        {
            this.View.gameObject.SetActive(true);
            this.Interactable = true;
        }

        public virtual void UtilityFinalize()
        {
            this.Interactable = false;
        }
    }
}
