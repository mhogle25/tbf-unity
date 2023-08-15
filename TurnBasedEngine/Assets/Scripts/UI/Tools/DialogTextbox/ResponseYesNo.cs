using System;

namespace BF2D.UI
{
    class ResponseYesNo : IResponseController
    {
        private readonly Action<bool> onConfirm;
        private readonly Action onBack;

        public ResponseYesNo(Action<bool> onConfirm, Action onBack)
        {
            this.onConfirm = onConfirm;
            this.onBack = onBack;
        }

        [Serializable]
        public class Data
        {
            public bool yes = true;
        }

        public void OnConfirm(string json)
        {
            Data data = Utilities.JSON.DeserializeString<Data>(json);

            if (data is not null)
                this.onConfirm?.Invoke(data.yes);

        }

        public void OnBack() => this.onBack?.Invoke();
    }
}