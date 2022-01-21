using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BF2D.UI
{
    public class PauseUtility : UIUtility
    {
        public static bool Paused { get { return PauseUtility.paused; } }
        private static bool paused = false;

        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_OnClick")]
        private UnityEvent onPause = new UnityEvent();

        private void Update()
        {
            if (InputManager.PausePress)
            {
                this.onPause.Invoke();
            }
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            PauseUtility.paused = true;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            PauseUtility.paused = false;
        }
    }

}
