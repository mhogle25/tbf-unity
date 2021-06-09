using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BF2D.UI;

namespace BF2D.Dialog {
    public class DialogTextbox : MonoBehaviour {

        [SerializeField] private Image textbox;
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private Image nametag;
        [SerializeField] private TextMeshProUGUI nametagTextField;
        [SerializeField] private UIOptionsGroup optionsGroup;

        [SerializeField] private Image continueArrow;

        [SerializeField] private List<TextAsset> dialogFiles = new List<TextAsset>();
        private Dictionary<string, List<string>> _dialogFiles = new Dictionary<string, List<string>>();

        public static DialogTextbox Instance { get { return _instance; } }
        private static DialogTextbox _instance;

        public float MessageSpeed { set { _messageSpeed = value; } }
        private float _messageSpeed = 0.05f;

        private void Awake() {
            //Setup of Monobehaviour Singleton
            if (_instance != this && _instance != null) {
                Destroy(_instance.gameObject);
            }
            _instance = this;

            LoadDialogFiles();
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

            Debug.Log("[Textbox] Dialog Files Loaded");
        }

        public void Message(string message) {
            textbox.gameObject.SetActive(true);

            List<string> lines = new List<string>();
            lines.Add(message);

            StartCoroutine(MessageParseAndDisplay(lines, 0, 0, _messageSpeed));
        }

        public void Dialog(string key, int index) {
            textbox.gameObject.SetActive(true);

            Debug.Log("[Textbox] Loading Dialog\nkey: " + key + ", index: " + index);

            List<string> lines = _dialogFiles[key];
            Debug.Log("[Textbox] Dialog Loaded\n" + lines.Count + " lines");
            StartCoroutine(MessageParseAndDisplay(lines, index, 0, _messageSpeed));
        }

        public void DialogManual(List<string> lines, int index) {
            textbox.gameObject.SetActive(true);

            StartCoroutine(MessageParseAndDisplay(lines, index, 0, _messageSpeed));
        }

        /// <summary>
        /// Parses and displays a dialog with the textbox
        /// </summary>
        /// <param name="lines">The dialog</param>
        /// <param name="dialogIndex">Which line of dialog to start on</param>
        /// <param name="messageIndex">Which character to start on in the current message</param>
        /// <param name="speed">The current message speed</param>
        /// <returns>Returns an iterator</returns>
        /// <returns>Returns an iterator</returns>
        private IEnumerator MessageParseAndDisplay(List<string> lines, int dialogIndex, int messageIndex, float speed) {
            //Set message to the current line of dialog
            string message = lines[dialogIndex];

            //If our message index is greater than the length of the message
            if (message.Length <= messageIndex) {
                StartCoroutine(Eol(lines, dialogIndex + 1, 0, speed));
                yield break;
            }

            //Begin tag parsing
            if (message[messageIndex] == '[') {
                //Take and read tag
                char tag = message[messageIndex + 1];
                int newMessageIndex = messageIndex;
                switch (tag) {
                    case 'P':   //Pause for seconds
                        yield return new WaitForSeconds(float.Parse(ParseTag(message, ref newMessageIndex)));
                        StartCoroutine(MessageParseAndDisplay(lines, dialogIndex, newMessageIndex + 1, speed));
                        break;
                    case 'S':   //New text speed
                        float newSpeed = float.Parse(ParseTag(message, ref newMessageIndex));
                        newSpeed = newSpeed >= 0 ? newSpeed : _messageSpeed; //If the new speed is less than 0, set it to the default speed
                        StartCoroutine(MessageParseAndDisplay(lines, dialogIndex, newMessageIndex + 1, newSpeed));
                        break;
                    case 'N':   //Orator name
                        string name = ParseTag(message, ref newMessageIndex);
                        NametagEnable(name);
                        StartCoroutine(MessageParseAndDisplay(lines, dialogIndex, newMessageIndex + 1, speed));
                        break;
                    case 'J':
                        int newDialogIndex = int.Parse(ParseTag(message, ref newMessageIndex));
                        StartCoroutine(MessageParseAndDisplay(lines, newDialogIndex - 1, 0, speed));
                        break;
                    case 'E':
                        StartCoroutine(Eof());
                        break;
                    default:
                        Debug.Log("[Textbox] Tag was not a valid character");
                        break;
                }
            } else { //Basic character
                string currentMessage = textField.text;
                currentMessage = currentMessage + message[messageIndex];
                textField.text = currentMessage;

                yield return new WaitForSeconds(speed);

                //Recurse
                StartCoroutine(MessageParseAndDisplay(lines, dialogIndex, messageIndex + 1, speed));
            }
        }

        private string ParseTag(string message, ref int index) {
            //Move to tag
            index++;
            //Move to colon, check if colon exists, move on
            index++;
            if (message[index] != ':') {
                Debug.Log("[Textbox] Incorrect Syntax, add ':' after tag");
            }
            index++;

            char character = message[index];    //Initialize character index
            string valueString = "";    //Float before conversion

            while (character != ']') {
                valueString += character;
                character = message[++index];
            }

            return valueString;   //Convert parsed string to float
        }

        private void NametagEnable(string name) {
            nametag.gameObject.SetActive(true);
            nametagTextField.text = name;
        }

        private void NametagDisable() {
            nametag.gameObject.SetActive(false);
        }

        private IEnumerator Eof() {
            continueArrow.enabled = true;
            while (true) {
                if (InputManager.Confirm) {
                    break;
                }
                yield return null;
            }
            continueArrow.enabled = false;
            textField.text = "";
            NametagDisable();
            textbox.gameObject.SetActive(false);
        }

        private IEnumerator Eol (List<string> lines, int dialogIndex, int messageIndex, float speed) {
            continueArrow.enabled = true;
            while (true) {
                if (InputManager.Confirm) {
                    break;
                }
                yield return null;
            }
            continueArrow.enabled = false;
            textField.text = "";
            StartCoroutine(MessageParseAndDisplay(lines, dialogIndex, messageIndex, speed));
        }
    }
}
