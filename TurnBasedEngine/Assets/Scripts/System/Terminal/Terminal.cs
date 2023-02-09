using UnityEngine.Events;
using UnityEngine;

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
        }

        private void Update()
        {
            if (Input.GetKeyDown(this.enableKey))
            {
                InputManager.Instance.InputEnabled = !InputManager.Instance.InputEnabled;
                this.view.gameObject.SetActive(!this.view.gameObject.activeSelf);
                if (this.view.gameObject.activeSelf)
                    this.onEnable?.Invoke();
                else
                    this.onDisable?.Invoke();
            }
        }

        public void RunCommand(string command)
        {
            this.terminalIn.Commit(command);
        }

        public void LogQuiet(string message)
        {
            this.terminalOut.Log(message);
        }

        public void Log(string message)
        {
            Debug.Log(message);
            LogQuiet(message);
        }

        public void LogWarningQuiet(string warning)
        {
            this.terminalOut.LogWarning(warning);
        }

        public void LogWarning(string warning)
        {
            Debug.LogWarning(warning);
            LogWarningQuiet(warning);
        }

        public void LogErrorQuiet(string error)
        {
            this.terminalOut.LogError(error);
        }

        public void LogError(string error)
        {
            Debug.LogError(error);
            LogErrorQuiet(error);
        }

        public void Clear()
        {
            this.terminalOut.Clear();
        }
    }
}