using UnityEngine.Events;
using UnityEngine;

namespace BF2D
{
    public class Terminal : MonoBehaviour
    {
        public static Terminal IO { get { return Terminal.instance; } }
        private static Terminal instance = null;

        [SerializeField] private RectTransform view = null;
        [SerializeField] private TerminalOutput terminalOut = null;
        [SerializeField] private TerminalInput terminalIn = null;

        [SerializeField] private UnityEvent onEnable = new();
        [SerializeField] private KeyCode enableKey = KeyCode.BackQuote;

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
                this.onEnable?.Invoke();
            }
        }

        public void RunCommand(string command)
        {
            this.terminalIn.Commit(command);
        }

        public void Log(string message)
        {
            Debug.Log(message);
            this.terminalOut.Log(message);
        }

        public void LogWarning(string warning)
        {
            Debug.LogWarning(warning);
            this.terminalOut.LogWarning(warning);
        }

        public void LogError(string error)
        {
            Debug.LogError(error);
            this.terminalOut.LogError(error);
        }

        public void Clear()
        {
            this.terminalOut.Clear();
        }
    }
}