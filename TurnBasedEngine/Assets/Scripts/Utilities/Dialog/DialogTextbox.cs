using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BF2D.UI;

namespace BF2D.Dialog {
    public class DialogTextbox : MonoBehaviour {
        private struct DialogData
        {
            public List<string> dialog;
            public int index;
        };

        [Header("Private References")]
        //Serialized private variables
        [SerializeField] private RectTransform _textbox = null;
        [SerializeField] private TextMeshProUGUI _textField = null;
        [SerializeField] private RectTransform _nametag = null;
        [SerializeField] private TextMeshProUGUI _nametagTextField = null;
        [SerializeField] private UIOptionsGrid _optionsGroup = null;
        [SerializeField] private Image _continueArrow = null;
        [SerializeField] private List<TextAsset> _dialogFiles = new List<TextAsset>();

        [Header("Public Preferences")]
        //Public variables
        public float DefaultMessageSpeed = 0.05f;
        public bool MessageInterrupt = false;

        [Header("Audio Preferences")]
        [SerializeField] private AudioSource _audioSource = null;

        //Getter Setters and their private variables
        public static DialogTextbox Instance { get { return _instance; } }
        private static DialogTextbox _instance;

        //Loaded dialogs
        private Dictionary<string, List<string>> _dialogs = new Dictionary<string, List<string>>();

        //The state delegate
        private Action _state;

        //The dialog queue
        private Queue<DialogData> _dialogQueue = new Queue<DialogData>();

        //Control Variables
        private List<string> _activeLines;
        private float _timeAccumulator = 0f;
        private float _messageSpeed = 0f;
        private int _dialogIndex = 0;
        private int _messageIndex = 0;

        private void Awake() {
            //Setup of Monobehaviour Singleton
            if (_instance != this && _instance != null) {
                Destroy(_instance.gameObject);
            }
            _instance = this;

            LoadDialogFiles();

            _state = DialogQueueHandler;
        }

        private void Update() {
            //Execute the current state of the dialog component
            _state();
        }

        #region Public Methods
        public void Message(string message) {
            List<string> lines = new List<string>
            {
                message + "[E]"
            };

            DialogData dialogData = new DialogData
            {
                dialog = lines,
                index = 0
            };

            _dialogQueue.Enqueue(dialogData);
        }

        public void Dialog(string key, int dialogIndex) {
            Debug.Log("[DialogTextbox] Loading Dialog\nkey: " + key + ", index: " + dialogIndex);

            DialogData dialogData = new DialogData
            {
                dialog = _dialogs[key],
                index = dialogIndex - 1
            };

            _dialogQueue.Enqueue(dialogData);
        }

        public void Dialog(List<string> lines, int dialogIndex) {
            DialogData dialogData = new DialogData
            {
                dialog = lines,
                index = dialogIndex - 1
            };

            _dialogQueue.Enqueue(dialogData);
        }
        #endregion

        #region States
        private void DialogQueueHandler()
        {
            if (_dialogQueue.Count > 0)
            {
                _textbox.gameObject.SetActive(true);

                DialogData dialogData = _dialogQueue.Dequeue();

                ResetControlVariables(dialogData.index);
                _activeLines = dialogData.dialog;

                Debug.Log("[DialogTextbox] Dialog Loaded\n" + _activeLines.Count + " lines");

                _state = MessageParseAndDisplayClocked;
            }
        }

        private void MessageParseAndDisplayClocked() {
            //Message Interrupts
            if (InputManager.ConfirmPress && MessageInterrupt) {                                    //If the confirm button is pressed and interrupt is on, switch to instantaneous parse
                MessageParseAndDisplayInstantaneous();                                              
                return;
            }

            //Message Parse Statement
            if (Time.time > _timeAccumulator) {
                _timeAccumulator = Time.time + _messageSpeed;                                       //Implement time increment
                MessageParseAndDisplay();                                                           //Call the message parse and display of the next character or implementation of the next flag
            }
        }

        private void EndOfLine() {
            if (!_continueArrow.enabled)
                _continueArrow.enabled = true;

            if (InputManager.ConfirmPress) {
                _continueArrow.enabled = false;
                _textField.text = "";
                _dialogIndex++;                         //Increment dialog index to the next line of dialog
                _messageIndex = 0;                      //Reset the message index to be on the first character of the line
                _state = MessageParseAndDisplayClocked; //Change the state to MessageParseAndDisplay
            }
        }

        private void EndOfDialog() {
            if (!_continueArrow.enabled)
                _continueArrow.enabled = true;

            if (InputManager.ConfirmPress) {
                _continueArrow.enabled = false;
                _textField.text = "";
                NametagDisable();
                //Reset the State
                _state = DialogQueueHandler;
                _textbox.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Private Methods
        private void LoadDialogFiles() {
            foreach (TextAsset file in _dialogFiles) {

                List<string> lines = new List<string>();
                StringReader lineReader = new StringReader(file.text);
                string line;
                while (true) {
                    line = lineReader.ReadLine();
                    if (line == null) {
                        break;
                    }
                    lines.Add(line);
                }

                _dialogs[file.name] = lines;
            }

            Debug.Log("[DialogTextbox] Dialog Files Loaded");
        }

        private void ResetControlVariables(int dialogIndex) {
            _dialogIndex = dialogIndex;
            _messageIndex = 0;
            _timeAccumulator = 0f;
            _messageSpeed = DefaultMessageSpeed;
            _activeLines = null;
        }

        private void MessageParseAndDisplayInstantaneous() {
            while (MessageParseAndDisplay());                                                       //Run parse and display until the end of the line or end of dialog
            _timeAccumulator = 0f;                                                                  //Reset the time accumulator
        }

        private bool MessageParseAndDisplay() {
            string message = _activeLines[_dialogIndex];                                            //Set message to the current line of dialog

            //If our message index is greater than the length of the message
            if (message.Length <= _messageIndex) {
                //Change the state to Eol
                _state = EndOfLine;

                return false;
            }

            //Begin tag parsing
            if (message[_messageIndex] == '[') {
                //Take and read tag
                char tag = message[_messageIndex + 1];
                int newMessageIndex = _messageIndex;
                switch (tag) {
                    case 'P':                                                                       //Case: Pause for seconds
                        float wait = float.Parse(ParseTag(message, ref newMessageIndex));           //Add a pause to the time accumulator
                        _timeAccumulator += wait;
                        _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case 'S':                                                                       //Case: New text speed
                        float newSpeed = float.Parse(ParseTag(message, ref newMessageIndex));
                        newSpeed = newSpeed >= 0 ? newSpeed : DefaultMessageSpeed;                  //If the new speed is less than 0, set it to the default speed
                        _messageSpeed = newSpeed;
                        _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case 'N':                                                                       //Case: Orator name
                        string name = ParseTag(message, ref newMessageIndex);
                        if (name == "-1")
                        {
                            NametagDisable();
                        } else
                        {
                            NametagEnable(name);
                        }
                        _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case 'J':                                                                       //Case: Jump
                        int newDialogIndex = int.Parse(ParseTag(message, ref newMessageIndex));
                        _dialogIndex = newDialogIndex - 1;
                        _messageIndex = 0;
                        break;
                    case 'E':
                        _state = EndOfDialog;
                        return false;
                    default:
                        Debug.Log("[DialogTextbox] Tag was not a valid character");
                        break;
                }
            } else { //Basic character
                string currentMessage = _textField.text;
                currentMessage = currentMessage + message[_messageIndex];
                _textField.text = currentMessage;

                _messageIndex++;                                                                    //Increment message index to move to next character
            }

            return true;
        }

        private string ParseTag(string message, ref int index) {
            //Move to tag
            index++;
            //Move to colon, check if colon exists, move on
            index++;
            if (message[index] != ':') {
                Debug.Log("[DialogTextbox] Incorrect Syntax, add ':' after tag");
            }
            index++;

            char character = message[index];    //Initialize character index
            string valueString = "";            //Float before conversion

            while (character != ']') {
                valueString += character;
                character = message[++index];
            }

            return valueString;                 //Convert parsed string to float
        }

        private void NametagEnable(string name) {
            _nametag.gameObject.SetActive(true);
            _nametagTextField.text = name;
        }

        private void NametagDisable() {
            _nametag.gameObject.SetActive(false);
        }
        #endregion
    }
}