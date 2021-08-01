using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BF2D.UI;

namespace BF2D.Dialog {
    public class DialogTextbox : MonoBehaviour {
        [Header("Private references")]
        //Serialized private variables
        [SerializeField] private Image textbox;
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private Image nametag;
        [SerializeField] private TextMeshProUGUI nametagTextField;
        [SerializeField] private UIOptionsGroup optionsGroup;

        [SerializeField] private Image continueArrow;

        [SerializeField] private List<TextAsset> dialogFiles = new List<TextAsset>();

        [Header("Public options")]
        //Public variables
        public float DefaultMessageSpeed = 0.05f;

        //Getter Setters and their private variables
        public static DialogTextbox Instance { get { return _instance; } }
        private static DialogTextbox _instance;

        //Internal private variables
        private Dictionary<string, List<string>> _dialogFiles = new Dictionary<string, List<string>>();

        private Action _state;

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
        }

        private void LateUpdate() {
            //Execute the current state of the dialog component
            if (_state != null)
                _state();
        }

        private void LoadDialogFiles() {
            foreach (TextAsset file in dialogFiles) {

                List<string> lines = new List<string>();
                StringReader lineReader = new StringReader(file.text);
                string line;
                while(true) {
                    line = lineReader.ReadLine();
                    if (line == null) {
                        break;
                    }
                    lines.Add(line);
                }

                _dialogFiles[file.name] = lines;
            }

            Debug.Log("[DialogTextbox] Dialog Files Loaded");
        }

        public bool Message(string message) {
            if (_state == null) {
                textbox.gameObject.SetActive(true);

                ResetControlVariables(0);

                _activeLines = new List<string>();
                _activeLines.Add(message);

                _state += MessageParseAndDisplay;

                return true;
            } else {
                return false;
            }
        }

        public bool Dialog(string key, int index) {
            if (_state == null) {
                textbox.gameObject.SetActive(true);

                ResetControlVariables(index);
                Debug.Log("[DialogTextbox] Loading Dialog\nkey: " + key + ", index: " + index);

                _activeLines = _dialogFiles[key];
                Debug.Log("[DialogTextbox] Dialog Loaded\n" + _activeLines.Count + " lines");

                _state += MessageParseAndDisplay;
                return true;
            } else {
                return false;
            }
        }

        public bool DialogManual(List<string> lines, int index) {
            if (_state == null) {
                textbox.gameObject.SetActive(true);

                ResetControlVariables(index);

                _activeLines = lines;

                _state += MessageParseAndDisplay;

                return true;
            } else {
                return false;
            }
        }

        private void MessageParseAndDisplay() {
            //Message Interrupts
            if (InputManager.Confirm) {
                MessageParseAndDisplayInstantaneous();
                return;
            }

            //Message Parse Statement
            if (Time.time > _timeAccumulator) {
                _timeAccumulator = Time.time + _messageSpeed;                                           //Implement time increment

                string message = _activeLines[_dialogIndex];                                            //Set message to the current line of dialog

                //If our message index is greater than the length of the message
                if (message.Length <= _messageIndex) {
                    //Change the state to Eol
                    _state -= MessageParseAndDisplay;
                    _state += EndOfLine;

                    return;
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
                            NametagEnable(name);
                            _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                            break;
                        case 'J':                                                                       //Case: Jump
                            int newDialogIndex = int.Parse(ParseTag(message, ref newMessageIndex));
                            _dialogIndex = newDialogIndex - 1;
                            _messageIndex = 0;
                            break;
                        case 'E':                                                                       //Case: End of Dialog
                            //Change the state to Eof
                            _state -= MessageParseAndDisplay;
                            _state += EndOfDialog;
                            break;
                        default:
                            Debug.Log("[DialogTextbox] Tag was not a valid character");
                            break;
                    }
                } else { //Basic character
                    string currentMessage = textField.text;
                    currentMessage = currentMessage + message[_messageIndex];
                    textField.text = currentMessage;

                    _messageIndex++;                                                                 //Increment message index to move to next character
                }
            }
        }

        private void MessageParseAndDisplayInstantaneous() {

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
            nametag.gameObject.SetActive(true);
            nametagTextField.text = name;
        }

        private void NametagDisable() {
            nametag.gameObject.SetActive(false);
        }

        private void EndOfDialog() {
            if (!continueArrow.enabled)
                continueArrow.enabled = true;

            if (InputManager.Confirm) {
                continueArrow.enabled = false;
                textField.text = "";
                NametagDisable();
                //Reset the State
                _state -= EndOfDialog;           
                textbox.gameObject.SetActive(false);
            }
        }

        private void EndOfLine() {
            if (!continueArrow.enabled)    
                continueArrow.enabled = true;
            
            if (InputManager.Confirm) {
                continueArrow.enabled = false;
                textField.text = "";
                _dialogIndex++;                  //Increment dialog index to the next line of dialog
                _messageIndex = 0;               //Reset the message index to be on the first character of the line
                _state -= EndOfLine;             //Change the state to MessageParseAndDisplay
                _state += MessageParseAndDisplay;
            }
        }

        private void ResetControlVariables(int dialogIndex) {
            _dialogIndex = dialogIndex;
            _messageIndex = 0;
            _timeAccumulator = 0f;
            _messageSpeed = DefaultMessageSpeed;
            _activeLines = null;
        }
    }
}
