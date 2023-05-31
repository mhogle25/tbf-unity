using UnityEngine.Events;
using UnityEngine;
using System;

namespace BF2D
{
    public class Terminal : MonoBehaviour
    {
        public static Terminal IO { get { return Terminal.instance; } }
        private static Terminal instance = null;

        [SerializeField] private KeyCode enableKey = KeyCode.BackQuote;
        [SerializeField] private RectTransform view = null;
        [SerializeField] private TerminalOutput terminalOut = null;
        [SerializeField] private TerminalInput terminalIn = null;

        [SerializeField] private UnityEvent onEnable = new();
        [SerializeField] private UnityEvent onDisable = new();

        private void Awake()
        {
            //Setup of Monobehaviour Singleton
            if (Terminal.instance != this && Terminal.instance != null)
                Destroy(Terminal.instance.gameObject);

            Terminal.instance = this;

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
            this.terminalOut.Clear();
        }

        public void Log(string message)
        {
            this.terminalOut.Log(message);
        }

        public void LogWarning(string warning)
        {
            this.terminalOut.LogWarning(warning);
        }

        public void LogError(string error)
        {
            SetViewActive(true);
            this.terminalOut.LogError(error);
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
            this.terminalIn.Commit(command);
        }

        public void RunCommand(string commandID, params string[] arguments)
        {
            this.terminalIn.Commit(commandID, arguments);
        }
        #endregion

        private void LogEvent(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error: DebugLogError(logString); return;
                case LogType.Assert: DebugLogWarning(logString); return;
                case LogType.Warning: DebugLogWarning(logString); return;
                case LogType.Log: DebugLog(logString); return;
                case LogType.Exception: DebugLogError(logString); return;
            }
        }

        private void DebugLog(string message)
        {
            Log(message);
        }

        private void DebugLogWarning(string warning)
        {
            LogWarning(warning);
        }

        private void DebugLogError(string error)
        {
            LogError(error);
        }

        private void SetViewActive(bool setActive)
        {
            InputManager.Instance.InputEnabled = !setActive;
            this.view.gameObject.SetActive(setActive);
            if (this.view.gameObject.activeSelf)
                this.onEnable?.Invoke();
            else
                this.onDisable?.Invoke();
        }
    }
}