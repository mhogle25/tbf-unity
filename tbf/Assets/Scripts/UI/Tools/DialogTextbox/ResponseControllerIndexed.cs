using System;

namespace BF2D.UI
{
    class ResponseControllerIndexed : IResponseController
    {
        private readonly Action<int> onConfirmIndex;
        private readonly Action<string> onConfirmKey;
        private readonly Action onBack;

        public ResponseControllerIndexed(Action<int> onConfirm, Action onBack)
        {
            this.onConfirmIndex = onConfirm;
            this.onBack = onBack;
        }

        public ResponseControllerIndexed(Action<string> onConfirm, Action onBack)
        {
            this.onConfirmKey = onConfirm;
            this.onBack = onBack;
        }

        public ResponseControllerIndexed(Action<int> onConfirmIndex, Action<string> onConfirmKey, Action onBack)
        {
            this.onConfirmIndex = onConfirmIndex;
            this.onConfirmKey = onConfirmKey;
            this.onBack = onBack;
        }

        [Serializable]
        public class ActionData
        {
            public int index = 0;
            public string key = string.Empty;
        }

        public void OnConfirm(string json)
        {
            ActionData data = Utilities.JSON.DeserializeJson<ActionData>(json);

            if (data is not null)
            {
                this.onConfirmIndex?.Invoke(data.index);
                this.onConfirmKey?.Invoke(data.key);
            }

        }

        public void OnBack() => this.onBack?.Invoke();
    }
}