using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace BF2D.UI
{
    public abstract class UIUtility : UIComponent
    {
        public RectTransform View { get { return this.view; } }
        [SerializeField] private protected RectTransform view = null;

        public void UtilityInitialize()
        {
            this.View.gameObject.SetActive(true);
            this.Interactable = true;
        }

        public void UtilityFinalize()
        {
            this.Interactable = false;
        }
    }
}
