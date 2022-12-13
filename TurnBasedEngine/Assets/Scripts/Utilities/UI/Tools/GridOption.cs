using BF2D;
using BF2D.Enums;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BF2D.UI
{
    public abstract class GridOption : InputEvents
    {
        public struct Data
        {
            public string name;
            public string text;
            public Sprite icon;
            public InputButtonCollection<Action> actions;
        };

        public Data GetData { get { return this.data; } }
        protected Data data;

        public abstract bool Setup(Data optionData);

        public abstract void SetCursor(bool status);

        public virtual void InvokeEvent(InputButton inputButton)
        {
            if (InputEventEnabled(inputButton))
                GetInputEvent(inputButton)?.Invoke();
        }
    }
}
