using BF2D.Enums;
using UnityEngine;

namespace BF2D.UI
{
    public class SpriteOption : GridOption
    {
        [SerializeField] private SpriteRenderer cursor = null;

        public override bool Setup(Data optionData)
        {
            throw new System.NotImplementedException();
        }

        public override void SetCursor(bool status)
        {
            this.cursor.enabled = status;
        }

        public override void InvokeEvent(InputButton inputButton)
        {
            base.InvokeEvent(inputButton);
        }
    }
}