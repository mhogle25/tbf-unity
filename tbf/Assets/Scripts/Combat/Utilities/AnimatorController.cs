using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game.Combat
{
    class AnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;

        public bool HasEvent => this.animEvent is not null;

        private string animState = Strings.Animation.IDLE_ID;
        public delegate List<string> RunEvent();
        private RunEvent animEvent = null;

        public void ChangeAnimState(string newState, RunEvent callback = null)
        {
            if (this.animState == newState) return;
            this.animator.Play(newState);
            this.animState = newState;
            this.animEvent = callback;
        }

        public List<string> InvokeAnimEvent()
        {
            List<string> dialog = null;
            dialog = this.animEvent?.Invoke();
            this.animEvent = null;
            return dialog;
        }
    }
}