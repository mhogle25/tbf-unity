using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Newtonsoft.Json;
using BF2D.Enums;

namespace BF2D.UI {
    public class DialogTextbox : UIUtility {
        private struct DialogData
        {
            public List<string> dialog;
            public int index;
            public Action callback;
        };

        [Serializable]
        private struct ResponseData
        {
            public string text;
            public int dialogIndex;
            public object action;
            public object prereq;
        };

        [Header("Private References")]
        //Serialized private variables
        [SerializeField] private TextMeshProUGUI textField = null;
        [SerializeField] private RectTransform nametag = null;
        [SerializeField] private TextMeshProUGUI nametagTextField = null;
        [SerializeField] private Image continueIcon = null;

        [Header("Dialog")]
        //Public variables
        [SerializeField] private string dialogFilesPath = string.Empty;
        public float DefaultMessageSpeed = 0.05f;
        public bool MessageInterrupt = false;
        public bool AutoPass = false;
        [SerializeField] private UnityEvent onEndOfQueuedDialogs = new();

        [Header("Dialog Responses")]
        public bool ResponseOptionsEnabled = true;
        [SerializeField] private string responseOptionsFilesPath = string.Empty;
        [SerializeField] private OptionsGridControl responseOptionsControl = null;
        [SerializeField] private GameCondition prereqConditionChecker = null;
        
        [Serializable] public class ResponseOptionEvent : UnityEvent<string> { }
        [SerializeField] private ResponseOptionEvent responseOptionEvent = new();

        [Header("Audio")]
        [SerializeField] private AudioClipCollection voiceCollection = null;
        [SerializeField] private AudioSource confirmAudioSource = null;
        [SerializeField] private AudioSource voiceAudioSource = null;
        [SerializeField] private AudioClip defaultVoice = null;

        //Loaded dialogs
        private readonly Dictionary<string, List<string>> dialogs = new();
        //Loaded dialog options
        private readonly Dictionary<string, string> dialogResponses = new();

        public Enums.UIState State
        {
            get
            {
                if (!this.interactable)
                    return Enums.UIState.Idle;
                if (this.state == DialogQueueHandler)
                    return Enums.UIState.Standby;
                if (this.state == MessageParseAndDisplayClocked)
                    return Enums.UIState.Running;
                if (this.state == EndOfLine)
                    return Enums.UIState.Standby;
                return Enums.UIState.Idle;
            }
        }
        //The state delegate
        private Action state = null;

        //The dialog queue
        private readonly Queue<DialogData> dialogQueue = new();

        //The current callback function
        private Action callback = null;

        //Control Variables
        private List<string> activeLines = null;
        private float timeAccumulator = 0f;
        private float messageSpeed = 0f;
        private int dialogIndex = 0;
        private int messageIndex = 0;
        private bool pass = false;
        private int nextDialogIndex = -1;
        private bool continueFlag = false;

        //Misc
        private const int defaultValue = -1;
        private const char pauseTag = 'P';
        private const char speedTag = 'S';
        private const char voiceTag = 'V';
        private const char jumpTag = 'J';
        private const char nameTag = 'N';
        private const char endTag = 'E';
        private const char responseTag = 'R';
        private const char insertTag = 'I';

        private void Awake() {
            LoadDialogFiles();
            LoadDialogResponseFiles();

            this.state = DialogQueueHandler;
        }

        private void Update()
        { 
            if (this.Interactable)
                this.state?.Invoke();
        }

        #region Public Methods
        /// <summary>
        /// Pushes a single message to the dialog queue
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        public void Message(string message) 
        {
            Message(message, null);
        }

        /// <summary>
        /// Pushes a single message to the dialog queue with a callback function
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <param name="callbackFunction">Called at the end of dialog</param>
        public void Message(string message, Action callbackFunction)
        {
            Message(message, callbackFunction, null);
        }

        /// <summary>
        /// Pushes a single message to the dialog queue with a callback function
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <param name="callbackFunction">Called at the end of dialog</param>
        public void Message(string message, Action callbackFunction, List<string> inserts)
        {
            List<string> lines = new List<string>
            {
                $"{message}[{DialogTextbox.endTag}]'"
            };

            lines = ReplaceInsertTags(lines, inserts);

            DialogData dialogData = new DialogData
            {
                dialog = lines,
                index = 0,
                callback = callbackFunction
            };

            this.dialogQueue.Enqueue(dialogData);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue
        /// </summary>
        /// <param name="key">The filename of the desired dialog</param>
        /// <param name="startingLineIndex">The line the dialog will start from (0 is the first line)</param>
        public void Dialog(string key, int startingLineIndex) 
        {
            Dialog(key, startingLineIndex, null);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue with a callback function
        /// </summary>
        /// <param name="key">The filename of the desired dialog</param>
        /// <param name="startingLineIndex">The line the dialog will start from (0 is the first line)</param>
        /// <param name="callbackFunction">Called at the end of dialog</param>
        public void Dialog(string key, int startingLineIndex, Action callbackFunction)
        {
            Dialog(key, startingLineIndex, callbackFunction, null);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue with a callback function
        /// </summary>
        /// <param name="key">The filename of the desired dialog</param>
        /// <param name="startingLineIndex">The line the dialog will start from (0 is the first line)</param>
        /// <param name="callbackFunction">Called at the end of dialog</param>
        public void Dialog(string key, int startingLineIndex, Action callbackFunction, List<string> inserts)
        {
            //Debug.Log("[DialogTextbox] Loading Dialog\nkey: " + key + ", index: " + dialogIndex);

            if (!this.dialogs.ContainsKey(key))
            {
                Debug.LogError($"[DialogTextbox] The key '{key}' was not found in the dialogs dictionary");
                return;
            }

            List<string> lines = this.dialogs[key];

            lines = ReplaceInsertTags(lines, inserts);

            DialogData dialogData = new DialogData
            {
                dialog = lines,
                index = startingLineIndex,
                callback = callbackFunction
            };

            this.dialogQueue.Enqueue(dialogData);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="dialogIndex">The line the dialog starts from (0 is the first line)</param>
        public void Dialog(List<string> lines, int dialogIndex) 
        {
            Dialog(lines, dialogIndex, null);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="startingLineIndex">The line the dialog starts from (0 is the first line)</param>
        /// <param name="callbackFunction">Called at the end of dialog</param>
        public void Dialog(List<string> lines, int startingLineIndex, Action callbackFunction)
        {
            Dialog(lines, startingLineIndex, callbackFunction, null);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="startingLineIndex">The line the dialog starts from (0 is the first line)</param>
        /// <param name="callbackFunction">Called at the end of dialog</param>
        public void Dialog(List<string> lines, int startingLineIndex, Action callbackFunction, List<string> inserts)
        {
            if (lines is null)
            {
                Debug.LogError("[DialogTextbox] Tried to queue a dialog but the dialog was null");
            }

            List<string> newLines = ReplaceInsertTags(lines, inserts);
            DialogData dialogData = new()
            {
                dialog = newLines,
                index = startingLineIndex,
                callback = callbackFunction
            };

            this.dialogQueue.Enqueue(dialogData);
        }

        /// <summary>
        /// Continues to the next line of dialog and interrupts if MessageInterrupt is enabled
        /// </summary>
        public void Continue()
        {
            if (!this.Interactable)
                return;

            if (this.state == MessageParseAndDisplayClocked && MessageInterrupt) // If the confirm button is pressed and interrupt is on, switch to instantaneous parse
            {
                MessageParseAndDisplayInstantaneous();
            }
            else if (this.state == EndOfLine || this.state == EndOfDialog)
            {
                this.continueFlag = true;
            }
        }

        public void LoadDialogFiles()
        {
            this.dialogs.Clear();
            BF2D.Utilities.TextFile.LoadTextFiles(this.dialogs, Path.Combine(Application.streamingAssetsPath, this.dialogFilesPath));
        }

        public void LoadDialogResponseFiles()
        {
            this.dialogResponses.Clear();
            BF2D.Utilities.TextFile.LoadTextFiles(this.dialogResponses, Path.Combine(Application.streamingAssetsPath, this.responseOptionsFilesPath));
        }
        #endregion

        #region Public Static Utilities
        public static List<string> ReplaceInsertTags(List<string> dialog, List<string> inserts)
        {
            if (inserts is null)
                return dialog;
            if (inserts.Count < 1)
                return dialog;

            List<string> newDialog = dialog;
            for (int i = 0; i < newDialog.Count; i++)
            {
                newDialog[i] = ReplaceInsertTags(newDialog[i], inserts);
            }
            return newDialog;
        }
        #endregion

        #region States
        private void DialogQueueHandler()
        {
            if (this.dialogQueue.Count > 0) {

                DialogData dialogData = this.dialogQueue.Dequeue();

                ResetControlVariables(dialogData.index);
                this.voiceAudioSource.clip = this.defaultVoice;
                this.activeLines = dialogData.dialog;
                this.callback = dialogData.callback;
                this.textField.text = "";

                //Debug.Log("[DialogTextbox] Dialog Loaded\n" + this.activeLines.Count + " lines");

                this.state = MessageParseAndDisplayClocked;
            }
        }

        private void MessageParseAndDisplayClocked() {
            //Message Parse Statement
            if (Time.time > this.timeAccumulator) {
                this.timeAccumulator = Time.time + this.messageSpeed;       //Implement time increment
                MessageParseAndDisplay();                                   //Call the message parse and display of the next character or implementation of the next flag
            }
        }

        private void EndOfLine() {
            if (!this.continueIcon.enabled)
                this.continueIcon.enabled = true;

            if (this.AutoPass)
                Continue();

            if (this.continueFlag || this.pass) {
                this.continueFlag = false;
                this.pass = false;
                BF2D.Utilities.Audio.PlayAudioSource(this.confirmAudioSource);       //Play the confirm sound
                this.continueIcon.enabled = false;
                this.textField.text = "";
                if (this.nextDialogIndex != DialogTextbox.defaultValue)
                {
                    this.dialogIndex = this.nextDialogIndex;
                    this.nextDialogIndex = DialogTextbox.defaultValue;
                } else
                {
                    this.dialogIndex++;                     //Increment dialog index to the next line of dialog
                }
                this.messageIndex = 0;                      //Reset the message index to be on the first character of the line
                this.state = MessageParseAndDisplayClocked; //Change the state to MessageParseAndDisplay
            }
        }

        private void EndOfDialog() {
            if (this.nextDialogIndex != DialogTextbox.defaultValue)
                this.state = EndOfLine;

            if (!this.continueIcon.enabled)
                this.continueIcon.enabled = true;

            if (this.AutoPass)
                Continue();

            if (this.continueFlag || this.pass) {
                this.pass = false;
                BF2D.Utilities.Audio.PlayAudioSource(this.confirmAudioSource);   //Play the confirm sound
                this.continueIcon.enabled = false;
                NametagDisable();
                //Reset the State
                this.state = DialogQueueHandler;
                //Call the callback function if it exists
                this.callback?.Invoke();
                this.callback = null;
                //Call the EOD event
                if (this.dialogQueue.Count < 1)
                {
                    this.onEndOfQueuedDialogs?.Invoke();
                    return;
                }
            }
        }
        #endregion

        #region Private Methods
        private void ResetControlVariables(int dialogIndex) {
            this.dialogIndex = dialogIndex;
            this.messageIndex = 0;
            this.timeAccumulator = 0f;
            this.messageSpeed = DefaultMessageSpeed;
            this.activeLines = null;
            this.pass = false;
            this.nextDialogIndex = -1;
            this.continueFlag = false;
        }

        private void MessageParseAndDisplayInstantaneous() {
            while (MessageParseAndDisplay());   //Setup parse and display until end of line, end of dialog, or option response is called
            this.timeAccumulator = 0f;
        }

        private bool MessageParseAndDisplay() {
            string message = this.activeLines[this.dialogIndex];    //Set message to the current line of dialog

            //If our message index is greater than the length of the message
            if (message.Length <= this.messageIndex) {
                //Change the state to Eol
                this.state = EndOfLine;

                return false;
            }

            //Begin tag parsing
            if (message[this.messageIndex] == '[') {
                //Take and read tag
                char tag = message[this.messageIndex + 1];
                int newMessageIndex = this.messageIndex;
                switch (tag) {
                    case DialogTextbox.pauseTag:                                                    //Case: Pause for seconds
                        float wait = float.Parse(ParseTag(message, ref newMessageIndex));           //Add a pause to the time accumulator
                        this.timeAccumulator += wait;
                        this.messageIndex = newMessageIndex + 1;                                    //Increment the message index accordingly
                        break;
                    case DialogTextbox.speedTag:                                                    //Case: New text speed
                        float newSpeed = float.Parse(ParseTag(message, ref newMessageIndex));
                        newSpeed = newSpeed >= 0 ? newSpeed : DefaultMessageSpeed;                  //If the new speed is less than 0, set it to the default speed
                        this.messageSpeed = newSpeed;
                        this.messageIndex = newMessageIndex + 1;                                    //Increment the message index accordingly
                        break;
                    case DialogTextbox.nameTag:                                                     //Case: Orator name
                        string name = ParseTag(message, ref newMessageIndex);
                        if (name == DialogTextbox.defaultValue.ToString())
                        {
                            NametagDisable();
                        } 
                        else
                        {
                            NametagEnable(name);
                        }
                        this.messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case DialogTextbox.jumpTag:                                                         //Case: Jump
                        int newDialogIndex = int.Parse(ParseTag(message, ref newMessageIndex));
                        this.dialogIndex = newDialogIndex;
                        this.messageIndex = 0;
                        break;
                    case DialogTextbox.voiceTag:                                                        //Case: Voice
                        string key = ParseTag(message, ref newMessageIndex);

                        if (this.voiceCollection.Contains(key))
                        {
                            this.voiceAudioSource.clip = this.voiceCollection.Get(key);
                        } 
                        else if (key == DialogTextbox.defaultValue.ToString())
                        {
                            this.voiceAudioSource.clip = this.defaultVoice;
                        } 
                        else
                        {
                            Debug.LogError($"[DialogTextbox] Voice key '{key}' was not found in the voices dictionary");
                        }
                        this.messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
                        break;
                    case DialogTextbox.responseTag:
                        string data = ParseTag(message,ref newMessageIndex);
                        if (!string.IsNullOrEmpty(data))
                        {
                            List<ResponseData> options = new List<ResponseData>();

                            //Retrieve the data using Json Utility
                            if (ValidJson(data))   //If it looks like a JSON, try to deserialize it
                            {
                                // Debug.Log("[DialogTextbox] Response option data is a JSON, deserializing...");
                                options = DeserializeResponseData(data);
                            } 
                            else
                            {   //else, try using it as a key in the dialog options dictionary and deserialize its value
                                //Debug.Log("[DialogTextbox] Response option data was not a JSON, retrieving JSON file by key...");
                                if (this.dialogResponses.ContainsKey(data))
                                {
                                    //Debug.Log("[DialogTextbox] JSON file retrieved, deserializing...");
                                    options = DeserializeResponseData(this.dialogResponses[data]);
                                } 
                                else
                                {
                                    Debug.LogError($"[Dialog Textbox] The dialog response file for the specified key '{data}' was not found");
                                }
                            }

                            if (this.ResponseOptionsEnabled)
                                SetupResponses(options);

                            this.messageIndex = newMessageIndex + 1;
                        } else
                        {
                            Debug.LogError("[DialogTextbox] The value for the response data cannot be null");
                        }
                        this.messageIndex = newMessageIndex + 1;
                        return false;
                    case DialogTextbox.endTag:
                        this.state = EndOfDialog;
                        return false;
                    default:
                        Debug.LogError($"[DialogTextbox] Tag '{tag}' was not a valid character");
                        break;
                }
            } else { //Basic character
                if (message[this.messageIndex] != ' ')
                {
                    BF2D.Utilities.Audio.PlayAudioSource(this.voiceAudioSource);
                }

                string currentMessage = this.textField.text;
                currentMessage = currentMessage + message[this.messageIndex];
                this.textField.text = currentMessage;

                this.messageIndex++;                                                                    //Increment message index to move to next character
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

            char character = message[index];                //Initialize character index
            string valueString = string.Empty;              //Float before conversion

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
            this.nametag.gameObject.SetActive(true);
            this.nametagTextField.text = name;
        }

        private void NametagDisable() {
            this.nametag.gameObject.SetActive(false);
        }

        private bool ValidJson(string json)
        {
            try
            {
                object rd = JsonConvert.DeserializeObject<object>(json, new Newtonsoft.Json.Converters.StringEnumConverter());
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
            List<ResponseData> responseData;
            try
            {
                responseData = JsonConvert.DeserializeObject<List<ResponseData>>(json, new Newtonsoft.Json.Converters.StringEnumConverter());
            }
            catch (Exception x)
            {
                throw x;
            }
            return responseData;
        }

        private void SetupResponses(List<ResponseData> options)
        {
            this.responseOptionsControl.ControlledOptionsGrid.Setup(1, options.Count);

            foreach (ResponseData option in options)
            {
                try
                {
                    if (this.prereqConditionChecker)
                    {
                        if (this.prereqConditionChecker.CheckCondition(option.prereq.ToString()))
                        {
                            continue;
                        }
                    }

                    this.responseOptionsControl.ControlledOptionsGrid.Add(new UIOption.Data
                    {
                        text = option.text,
                        actions = new InputButtonCollection<Action>
                        {
                            [InputButton.Confirm] = () =>
                            {
                                if (this.responseOptionEvent != null)
                                {
                                    this.responseOptionEvent.Invoke(option.action.ToString());
                                }
                                FinalizeResponse(option.dialogIndex);
                            }
                        }
                    }); 
                } 
                catch (Exception x)
                {
                    throw x;
                }
            }

            UIControlsManager.Instance.TakeControl(this.responseOptionsControl);
            this.responseOptionsControl.ControlledOptionsGrid.SetCursorAtHead();
        }

        private void FinalizeResponse(int dialogIndex)
        {
            if (dialogIndex != DialogTextbox.defaultValue)
            {
                this.nextDialogIndex = dialogIndex;
            }
            this.responseOptionsControl.gameObject.SetActive(false);
            this.pass = true;
            this.state = MessageParseAndDisplayClocked;
            UIControlsManager.Instance.PassControlBack();
        }
        #endregion

        #region Private Static Utilities
        private static string ReplaceInsertTags(string message, List<string> inserts)
        {
            if (inserts is null)
                return message;
            if (inserts.Count < 1)
                return message;

            string newMessage = message;
            for (int i = 0; i < inserts.Count; i++)
            {
                newMessage = ReplaceInsertTags(message, inserts[i], i);
            }
            return newMessage;
        }

        private static string ReplaceInsertTags(string message, string insert, int index)
        {
            if (insert is null)
                return message;

            string newMessage = message;
            newMessage = newMessage.Replace($"[{DialogTextbox.insertTag}:{index}]", insert);
            return newMessage;
        }
        #endregion
    }
}