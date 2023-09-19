using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace BF2D.UI
{
    public abstract class UIUtility : MonoBehaviour, IUIComponent
    {
        [Header("UIUtility")]
        [SerializeField] private Transform view = null;
        [SerializeField] private bool interactable = false;

        public Transform View => this.view;
        public bool Interactable { get => this.interactable; set => this.interactable = value; }

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
