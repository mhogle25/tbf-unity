using UnityEngine.Events;
using UnityEngine;

namespace BF2D
{
    public class ShCtx : MonoBehaviourSingleton<ShCtx>
    {
        [Header("Shell")]
        [SerializeField] private KeyCode enableKey = KeyCode.BackQuote;
        [SerializeField] private RectTransform view = null;
        [SerializeField] private ShOut shellOut = null;
        [SerializeField] private ShIn shellIn = null;

        [SerializeField] private UnityEvent onEnable = new();
        [SerializeField] private UnityEvent onDisable = new();

        protected sealed override void SingletonAwakened()
        {
            Application.logMessageReceived += LogEvent;
        }

        private void Update()
        {
            if (Input.GetKeyDown(this.enableKey))
                SetViewActive(!this.view.gameObject.activeSelf);
        }

        private void OnDestroy() => Application.logMessageReceived -= LogEvent;

        #region Logging
        public void ClearLogs()
        {
            this.shellOut.Clear();
        }

        public void Log(string message)
        {
            this.shellOut.Log(message);
        }

        public void LogWarning(string warning)
        {
            this.shellOut.LogWarning(warning);
        }

        public void LogError(string error)
        {
            SetViewActive(true);
            this.shellOut.LogError(error);
        }

        public void Log(int value)
        {
            Log($"{value}");
        }

        public void LogWarning(int value)
        {
            LogWarning($"{value}");
        }

        public void LogError(int value)
        {
            SetViewActive(true);
            LogError($"{value}");
        }
        #endregion

        #region Commands
        public void RunCommand(string command)
        {
            this.shellIn.Commit(command);
        }

        public void RunCommand(string commandID, params string[] arguments)
        {
            this.shellIn.Commit(commandID, arguments);
        }
        #endregion

        private void LogEvent(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error: LogError(logString); return;
                case LogType.Assert: LogWarning(logString); return;
                case LogType.Warning: LogWarning(logString); return;
                case LogType.Log: Log(logString); return;
                case LogType.Exception: LogError(logString); return;
            }
        }

        private void SetViewActive(bool setActive)
        {
            InputCtx.One.InputEnabled = !setActive;
            this.view.gameObject.SetActive(setActive);
            if (this.view.gameObject.activeSelf)
                this.onEnable?.Invoke();
            else
                this.onDisable?.Invoke();
        }
    }
}