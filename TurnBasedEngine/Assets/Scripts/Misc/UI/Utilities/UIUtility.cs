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

        private protected void PlayAudioSource(AudioSource audioSource)
        {
            if (audioSource == null)
                return;

            if (audioSource.clip != null)
                audioSource.Play();
        }
    }
}
