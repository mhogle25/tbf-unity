using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

namespace BF2D.UI
{
    public abstract class UIUtility : UIComponent
    {
        public RectTransform View { get { return this.view; } }
        [SerializeField] protected RectTransform view = null;

        public bool Interactable { get { return this.interactable; } set { this.interactable = value; } }
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
