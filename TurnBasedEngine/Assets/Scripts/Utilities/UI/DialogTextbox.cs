using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Newtonsoft.Json;

namespace BF2D.UI {
    public class DialogTextbox : UIUtility {
        private struct DialogData
        {
            public List<string> dialog;
            public int index;
        };

        private struct ResponseData
        {
            public string text;
            public int dialogIndex;
            public object action;
        };

        [Header("Private References")]
        //Serialized private variables
        [SerializeField] private RectTransform _textbox = null;
        [SerializeField] private TextMeshProUGUI _textField = null;
        [SerializeField] private RectTransform _nametag = null;
        [SerializeField] private TextMeshProUGUI _nametagTextField = null;
        [SerializeField] private Image _continueArrow = null;
        [SerializeField] private List<TextAsset> _dialogFiles = new List<TextAsset>();

        [Header("Preferences")]
        //Public variables
        public float DefaultMessageSpeed = 0.05f;
        public bool MessageInterrupt = false;

        [Header("Dialog Responses")]
        [SerializeField] private UIOptionsGrid _responseOptionsGrid = null;
        [SerializeField] private List<TextAsset> _dialogResponseFiles = new List<TextAsset>();
        [System.Serializable]
        public class ResponseOptionEvent : UnityEvent<string> { }
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_OnClick")]
        private ResponseOptionEvent _responseOptionEvent = new ResponseOptionEvent();

        [Header("Audio")]
        [SerializeField] private AudioSource _confirmAudioSource = null;
        [SerializeField] private AudioSource _voiceAudioSource = null;
        [SerializeField] private AudioClip _defaultVoice = null;
        [SerializeField] private List<AudioClip> _voiceAudioClipFiles = new List<AudioClip>();

        //Getter Setters and their private variables
        public static DialogTextbox Instance { get { return _instance; } }
        private static DialogTextbox _instance;

        //Loaded dialogs
        private Dictionary<string, List<string>> _dialogs = new Dictionary<string, List<string>>();
        //Loaded Voice Clips
        private Dictionary<string, AudioClip> _voices = new Dictionary<string, AudioClip>();
        //Loaded dialog options
        private Dictionary<string, string> _dialogOptions = new Dictionary<string, string>();

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
        private AudioClip _activeVoice = null;
        private bool _pass = false;
        private int _nextDialogIndex = -1;

        //Misc
        private const string _defaultValue = "-1";
        private const char _pauseTag = 'P';
        private const char _speedTag = 'S';
        private const char _voiceTag = 'V';
        private const char _nameTag = 'N';
        private const char _jumpTag = 'J';
        private const char _endTag = 'E';
        private const char _responseTag = 'R';

        private void Awake() {
            //Setup of Monobehaviour Singleton
            if (_instance != this && _instance != null) {
                Destroy(_instance.gameObject);
            }
            _instance = this;

            LoadDialogFiles();
            LoadVoiceAudioClipFiles();
            LoadDialogResponseFiles();

            _state = DialogQueueHandler;
        }

        private void Update() {
            //Execute the current state of the dialog component
            _state();
        }

        #region Public Methods
        /// <summary>
        /// Pushes a single message to the dialog queue
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        public void Message(string message) {
            List<string> lines = new List<string>
            {
                message + '[' + _endTag + ']'
            };

            DialogData dialogData = new DialogData
            {
                dialog = lines,
                index = 0
            };

            _dialogQueue.Enqueue(dialogData);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue
        /// </summary>
        /// <param name="key">The filename of the desired dialog</param>
        /// <param name="dialogIndex">The line the dialog will start from (0 is the first line)</param>
        public void Dialog(string key, int dialogIndex) {
            Debug.Log("[DialogTextbox] Loading Dialog\nkey: " + key + ", index: " + dialogIndex);

            if (!_dialogs.ContainsKey(key))
            {
                Debug.LogError("[DialogTextbox] The key '" + key + "' was not found in the dialogs dictionary");
                return;
            }

            DialogData dialogData = new DialogData
            {
                dialog = _dialogs[key],
                index = dialogIndex
            };

            _dialogQueue.Enqueue(dialogData);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="dialogIndex">The line the dialog starts from (0 is the first line)</param>
        public void Dialog(List<string> lines, int dialogIndex) {
            DialogData dialogData = new DialogData
            {
                dialog = lines,
                index = dialogIndex
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
                _activeVoice = _defaultVoice;
                _activeLines = dialogData.dialog;

                Debug.Log("[DialogTextbox] Dialog Loaded\n" + _activeLines.Count + " lines");

                _state = MessageParseAndDisplayClocked;
            }
        }

        private void MessageParseAndDisplayClocked() {
            //Message Interrupts
            if (InputManager.ConfirmPress && MessageInterrupt) {    //If the confirm button is pressed and interrupt is on, switch to instantaneous parse
                MessageParseAndDisplayInstantaneous();                                              
                return;
            }

            //Message Parse Statement
            if (Time.time > _timeAccumulator) {
                _timeAccumulator = Time.time + _messageSpeed;       //Implement time increment
                MessageParseAndDisplay();                           //Call the message parse and display of the next character or implementation of the next flag
            }
        }

        private void PauseMessageDisplay()
        {
            return;
        }

        private void EndOfLine() {
            if (!_continueArrow.enabled)
                _continueArrow.enabled = true;

            if (InputManager.ConfirmPress || _pass) {
                _pass = false;
                PlayAudioSource(_confirmAudioSource);       //Play the confirm sound
                _continueArrow.enabled = false;
                _textField.text = "";
                if (_nextDialogIndex != int.Parse(_defaultValue))
                {
                    _dialogIndex = _nextDialogIndex;
                    _nextDialogIndex = -1;
                } else
                {
                    _dialogIndex++;                         //Increment dialog index to the next line of dialog
                }
                _messageIndex = 0;                      //Reset the message index to be on the first character of the line
                _state = MessageParseAndDisplayClocked; //Change the state to MessageParseAndDisplay
            }
        }

        private void EndOfDialog() {
            if (_nextDialogIndex != int.Parse(_defaultValue))
                _state = EndOfLine;

            if (!_continueArrow.enabled)
                _continueArrow.enabled = true;

            if (InputManager.ConfirmPress || _pass) {
                _pass = false;
                PlayAudioSource(_confirmAudioSource);   //Play the confirm sound
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

            Debug.Log("[DialogTextbox] Dialog files loaded");
        }

        private void LoadVoiceAudioClipFiles()
        {
            foreach (AudioClip file in _voiceAudioClipFiles)
            {
                _voices[file.name] = file;
            }

            Debug.Log("[DialogTextbox] Voice audio clip files loaded");
        }

        private void LoadDialogResponseFiles()
        {
            foreach (TextAsset textAsset in _dialogResponseFiles)
            {
                _dialogOptions[textAsset.name] = textAsset.text;
            }

            Debug.Log("[DialogTextbox] Response files loaded");
        }

        private void ResetControlVariables(int dialogIndex) {
            _dialogIndex = dialogIndex;
            _messageIndex = 0;
            _timeAccumulator = 0f;
            _messageSpeed = DefaultMessageSpeed;
            _activeLines = null;
            _pass = false;
        }

        private void MessageParseAndDisplayInstantaneous() {
            while (MessageParseAndDisplay()) ;   //Run parse and display until end of line, end of dialog, or option response is called
            _timeAccumulator = 0f;
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
                    case _pauseTag:                                                                       //Case: Pause for seconds
                        float wait = float.Parse(ParseTag(message, ref newMessageIndex));           //Add a pause to the time accumulator
                        _timeAccumulator += wait;
                        _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case _speedTag:                                                                       //Case: New text speed
                        float newSpeed = float.Parse(ParseTag(message, ref newMessageIndex));
                        newSpeed = newSpeed >= 0 ? newSpeed : DefaultMessageSpeed;                  //If the new speed is less than 0, set it to the default speed
                        _messageSpeed = newSpeed;
                        _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case _nameTag:                                                                       //Case: Orator name
                        string name = ParseTag(message, ref newMessageIndex);
                        if (name == _defaultValue)
                        {
                            NametagDisable();
                        } else
                        {
                            NametagEnable(name);
                        }
                        _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case _jumpTag:                                                                       //Case: Jump
                        int newDialogIndex = int.Parse(ParseTag(message, ref newMessageIndex));
                        _dialogIndex = newDialogIndex;
                        _messageIndex = 0;
                        break;
                    case _voiceTag:                                                                       //Case: Voice
                        string key = ParseTag(message, ref newMessageIndex);

                        if (_voices.ContainsKey(key))
                        {
                            _activeVoice = _voices[key];
                        } else if (key == _defaultValue)
                        {
                            _activeVoice = _defaultVoice;
                        } else
                        {
                            Debug.LogError("[DialogTextbox] Voice key '" + key + "' was not found in the voices dictionary");
                        }
                        _messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case _responseTag:
                        string data = ParseTag(message,ref newMessageIndex);
                        if (!string.IsNullOrEmpty(data))
                        {
                            List<ResponseData> options = new List<ResponseData>();

                            //Retrieve the data using Json Utility
                            if (ValidJson(data))   //If it looks like a JSON, try to deserialize it
                            {
                                Debug.Log("[DialogTextbox] Response option data is a JSON, deserializing...");
                                options = DeserializeResponseData(data);
                            } else
                            {   //else, try using it as a key in the dialog options dictionary and deserialize its value
                                Debug.Log("[DialogTextbox] Response option data was not a JSON, retrieving JSON file by key...");
                                if (_dialogOptions.ContainsKey(data))
                                {
                                    Debug.Log("[DialogTextbox] JSON file retrieved, deserializing...");
                                    options = DeserializeResponseData(_dialogOptions[data]);
                                } else
                                {
                                    Debug.LogError("[Dialog Textbox] The dialog response file for the specified key '" + data + "' was not found");
                                }
                            }
                            SetupResponses(options);
                            _messageIndex = newMessageIndex + 1;
                        } else
                        {
                            Debug.LogError("[DialogTextbox] The value for the response data cannot be null");
                        }
                        _messageIndex = newMessageIndex + 1;
                        return false;
                    case _endTag:
                        _state = EndOfDialog;
                        return false;
                    default:
                        Debug.LogError("[DialogTextbox] Tag '" + tag + "' was not a valid character");
                        break;
                }
            } else { //Basic character
                if (message[_messageIndex] != ' ')
                {
                    PlayAudioSource(_voiceAudioSource);
                }

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
                Debug.LogError("[DialogTextbox] Incorrect Syntax, add ':' after tag");
            }
            index++;

            char character = message[index];    //Initialize character index
            string valueString = "";            //Float before conversion

            Stack<char> stack = new Stack<char>();
            while (character != ']' || stack.Count > 0) {
                valueString += character;

                if (character == '[')
                {
                    stack.Push(character);
                }

                if (character == ']')
                {
                    stack.Pop();
                }

                character = message[++index];
            }

            return valueString;                 
        }

        private void NametagEnable(string name) {
            _nametag.gameObject.SetActive(true);
            _nametagTextField.text = name;
        }

        private void NametagDisable() {
            _nametag.gameObject.SetActive(false);
        }

        private bool ValidJson(string json)
        {
            try
            {
                object rd = JsonConvert.DeserializeObject(json);

            } 
            catch (Exception x)
            {
                x.ToString();
                return false;
            }

            return true;
        }

        private List<ResponseData> DeserializeResponseData(string json)
        {
            List<ResponseData> responseData = new List<ResponseData>();
            try
            {
                responseData = JsonConvert.DeserializeObject<List<ResponseData>>(json);
            }
            catch (Exception x)
            {
                throw x;
            }
            return responseData;
        }

        private void SetupResponses(List<ResponseData> options)
        {
            _state = PauseMessageDisplay;
            _responseOptionsGrid.gameObject.SetActive(true);
            _responseOptionsGrid.Setup(1, options.Count);

            foreach (ResponseData option in options)
            {
                _responseOptionsGrid.Add(new UIOptionData
                {
                    text = option.text,
                    action = () =>
                    {
                        if (_responseOptionEvent != null)
                        {
                            _responseOptionEvent.Invoke(option.action.ToString());
                        }
                        FinalizeResponse(option.dialogIndex);
                    }
                });
            }

            _responseOptionsGrid.SetCursorAtHead();
        }

        private void FinalizeResponse(int dialogIndex)
        {
            if (dialogIndex != int.Parse(_defaultValue))
            {
                _nextDialogIndex = dialogIndex;
            }
            _responseOptionsGrid.gameObject.SetActive(false);
            _pass = true;
            _state = MessageParseAndDisplayClocked;
        }
        #endregion
    }
}