using BF2D;
using BF2D.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BF2D.UI
{
    public abstract class GridOption : InputEvents, IUIComponent
    {
        public struct Data
        {
            public string name;
            public string text;
            public Sprite icon;
            public InputButtonCollection<Action> actions;
        };

        public virtual bool Interactable { get { return this.interactable; } set { this.interactable = value; } }
        protected bool interactable = true;

        public abstract bool Setup(Data optionData);

        public abstract void SetCursor(bool status);

        public virtual void InvokeEvent(InputButton inputButton)
        {
            if (InputEventEnabled(inputButton))
                GetInputEvent(inputButton)?.Invoke();
        }
    }
}
