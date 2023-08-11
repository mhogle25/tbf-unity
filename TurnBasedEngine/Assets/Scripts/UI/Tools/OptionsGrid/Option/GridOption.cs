using BF2D.Enums;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace BF2D.UI
{
    public abstract class GridOption : InputEvents, IUIComponent
    {
        public struct Data
        {
            public string name;
            public string text;
            public Sprite icon;
            public InputButtonCollection<Action> onInput;
            public Action onNavigate;
        };

        [Header("Grid Option")]
        [SerializeField] private UnityEvent onNavigate = new();

        public virtual bool Interactable { get => this.interactable; set => this.interactable = value; }
        protected bool interactable = true;

        private Action onNavigateCallback = null;

        public virtual void Setup(Data data)
        {
            this.gameObject.name = data.name ?? this.gameObject.name;
            this.onNavigateCallback = data.onNavigate ?? this.onNavigateCallback;

            if (data.onInput is not null)
            {
                foreach (InputButton inputButton in Enum.GetValues(typeof(InputButton)))
                {
                    if (data.onInput[inputButton] is null)
                        continue;

                    GetInputEvent(inputButton).AddListener(data.onInput[inputButton].Invoke);
                }
            }
        }

        public abstract void SetCursor(bool status);

        public virtual void InvokeEvent(InputButton inputButton)
        {
            if (InputEventEnabled(inputButton))
                GetInputEvent(inputButton)?.Invoke();
        }

        public void OnNavigate()
        {
            this.onNavigate?.Invoke();
            this.onNavigateCallback?.Invoke();
        }
    }
}
