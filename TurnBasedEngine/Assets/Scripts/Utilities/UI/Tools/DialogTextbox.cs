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
        private class ResponseData
        {
            public string text = string.Empty;
            public int dialogIndex = 0;
            public object action = null;
            public object prereq = null;
        };

        [Header("Private References")]
        //Serialized private variables
        [SerializeField] private Utilities.TextField textField = null;
        [SerializeField] private RectTransform nametag = null;
        [SerializeField] private Utilities.TextField nametagTextField = null;
        [SerializeField] private Image continueIcon = null;

        [Header("Dialog")]
        [SerializeField] private Utilities.FileManager dialogFileManager = null;
        //Public variables
        public float DefaultMessageSpeed = 0.05f;
        public bool MessageInterrupt = false;
        public bool AutoPass = false;
        public UnityEvent OnEndOfQueuedDialogs { get { return this.onEndOfQueuedDialogs; } }
        [SerializeField] private UnityEvent onEndOfQueuedDialogs = new();

        [Header("Dialog Responses")]
        [SerializeField] private Utilities.FileManager responsesFileManager = null;
        public bool ResponseOptionsEnabled = true;
        [SerializeField] private OptionsGridControl responseOptionsControl = null;
        [SerializeField] private GameCondition prereqConditionChecker = null;
        
        [Serializable] public class ResponseOptionEvent : UnityEvent<string> { }
        public ResponseOptionEvent ResponseEvent { get { return this.responseOptionEvent; } }
        [SerializeField] private ResponseOptionEvent responseOptionEvent = new();

        public UnityEvent ResponseBackEvent { get { return this.responseOptionBackEvent; } }
        public bool ResponseBackEventEnabled
        {
            get
            {
                return this.responseOptionsControl.ControlledOptionsGrid.BackEnabled;
            }
            set
            {
                this.responseOptionsControl.ControlledOptionsGrid.BackEnabled = value;
            }
        }
        [SerializeField] private UnityEvent responseOptionBackEvent = new();

        [Header("Audio")]
        [SerializeField] private AudioClipCollection voiceCollection = null;
        [SerializeField] private AudioSource confirmAudioSource = null;
        [SerializeField] private AudioSource voiceAudioSource = null;
        [SerializeField] private AudioClip defaultVoice = null;
        [Header("Assets")]
        //File Manager
        //Loaded dialogs
        private Dictionary<string, List<string>> dialogs = new();
        //Loaded dialog options
        private Dictionary<string, string> dialogResponses = new();

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

        private void Awake() 
        {
            this.dialogs = this.dialogFileManager.LoadFilesLined();
            this.dialogResponses = this.responsesFileManager.LoadFiles();
        }

        private void Update()
        { 
            if (this.Interactable)
                this.state?.Invoke();
        }

        #region Public Overrides
        public override void UtilityInitialize()
        {
            base.UtilityInitialize();
            this.state = DialogQueueHandler;
        }
        #endregion

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
            if (string.IsNullOrEmpty(message))
            {
                Terminal.IO.LogError("[DialogTextbox] Tried to queue a message but the message was null or empty");
                return;
            }

            List<string> lines = new()
            {
                $"{message}[{DialogTextbox.endTag}]'"
            };

            lines = ReplaceInsertTags(lines, inserts);

            DialogData dialogData = new()
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
            if (!this.dialogs.ContainsKey(key))
            {
                Terminal.IO.LogError($"[DialogTextbox] The key '{key}' was not found in the dialogs dictionary");
                return;
            }

            List<string> lines = this.dialogs[key];

            lines = ReplaceInsertTags(lines, inserts);

            DialogData dialogData = new()
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
            if (lines is null || lines.Count < 1)
            {
                Terminal.IO.LogError("[DialogTextbox] Tried to queue a dialog but the dialog was null or empty");
                return;
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

        public void Cancel()
        {
            this.dialogQueue.Clear();
            this.state = DialogQueueHandler;
        }
        #endregion

        #region Public Static Utilities
        public static List<string> ReplaceInsertTags(List<string> dialog, List<string> inserts)
        {
            if (inserts is null)
                return dialog;
            if (inserts.Count < 1)
                return dialog;

            List<string> newDialog = new();
            for (int i = 0; i < dialog.Count; i++)
            {
                newDialog.Add(ReplaceInsertTags(dialog[i], inserts));
            }
            return newDialog;
        }
        #endregion

        #region States
        private void DialogQueueHandler()
        {
            if (this.dialogQueue.Count < 1)
                return;

            DialogData dialogData = this.dialogQueue.Dequeue();

            ResetControlVariables(dialogData.index);
            this.voiceAudioSource.clip = this.defaultVoice;
            this.activeLines = dialogData.dialog;
            this.callback = dialogData.callback;
            this.textField.text = "";

            //Terminal.IO.Log("[DialogTextbox] Dialog Loaded\n" + this.activeLines.Count + " lines");

            this.state = MessageParseAndDisplayClocked;
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
                } 
                else
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
                //Call the callback function if it exists
                this.callback?.Invoke();
                this.callback = null;
                //Call the EOD event
                if (this.dialogQueue.Count < 1)
                    this.onEndOfQueuedDialogs?.Invoke();
                //Reset the State
                this.state = DialogQueueHandler;
            }
        }
        #endregion

        #region Private Methods
        private void ResetControlVariables(int dialogIndex)
        {
            this.continueIcon.enabled = false;
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
            if (this.dialogIndex >= this.activeLines.Count)
            {
                Terminal.IO.LogError($"[DialogTextbox:MessageParseAndDisplay] Tried to parse but the dialog index was out of range. Did you forget to use an end tag? (Previous line: {this.activeLines[this.dialogIndex - 1]})");
                return false;
            }

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
                            Terminal.IO.LogError($"[DialogTextbox:MessageParseAndDisplay] Voice key '{key}' was not found in the voices dictionary");
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
                                // Terminal.IO.Log("[DialogTextbox] Response option data is a JSON, deserializing...");
                                options = DeserializeResponseData(data);
                            } 
                            else
                            {   //else, try using it as a key in the dialog options dictionary and deserialize its value
                                //Terminal.IO.Log("[DialogTextbox] Response option data was not a JSON, retrieving JSON file by key...");
                                if (this.dialogResponses.ContainsKey(data))
                                {
                                    //Terminal.IO.Log("[DialogTextbox] JSON file retrieved, deserializing...");
                                    options = DeserializeResponseData(this.dialogResponses[data]);
                                } 
                                else
                                {
                                    Terminal.IO.LogError($"[DialogTextbox:MessageParseAndDisplay] The dialog response file for the specified key '{data}' was not found");
                                }
                            }

                            if (this.ResponseOptionsEnabled)
                                SetupResponses(options);

                            this.messageIndex = newMessageIndex + 1;
                            //this.state = null;
                        }
                        else
                        {
                            Terminal.IO.LogError("[DialogTextbox:MessageParseAndDisplay] The value for the response data cannot be null");
                        }
                        this.messageIndex = newMessageIndex + 1;
                        return false;
                    case DialogTextbox.endTag:
                        this.state = EndOfDialog;
                        return false;
                    default:
                        if (tag == DialogTextbox.insertTag)
                            Terminal.IO.LogError($"[DialogTextbox:MessageParseAndDisplay] Message '{message}' has incorrectly formatted insert tags");
                        else
                            Terminal.IO.LogError($"[DialogTextbox:MessageParseAndDisplay] Tag '{tag}' was not a valid character");
                        Cancel();
                        return true;
                }
            } else { //Basic character
                if (message[this.messageIndex] != ' ')
                {
                    BF2D.Utilities.Audio.PlayAudioSource(this.voiceAudioSource);
                }

                string currentMessage = this.textField.text;
                currentMessage += message[this.messageIndex];
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
                Terminal.IO.LogError("[DialogTextbox:ParseTag] Incorrect Syntax, add ':' after tag");
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
                                this.responseOptionEvent?.Invoke(option.action.ToString());
                                ResponseConfirm(option.dialogIndex);
                            },
                            [InputButton.Back] = () =>
                            {
                                FinalizeResponse();
                                this.responseOptionBackEvent?.Invoke();
                            }
                        }
                    }); 
                } 
                catch (Exception x)
                {
                    throw x;
                }
            }

            UIControlsManager.Instance.StartPhantomControl(this.responseOptionsControl);
            this.responseOptionsControl.ControlledOptionsGrid.SetCursorToFirst();
        }

        private void ResponseConfirm(int dialogIndex)
        {
            if (dialogIndex != DialogTextbox.defaultValue)
            {
                this.nextDialogIndex = dialogIndex;
            }
            FinalizeResponse();
            this.pass = true;
            this.state = MessageParseAndDisplayClocked;
        }

        private void FinalizeResponse()
        {
            UIControlsManager.Instance.EndPhantomControl();
            this.responseOptionsControl.gameObject.SetActive(false);
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
                newMessage = ReplaceInsertTags(newMessage, inserts[i], i);
            }
            return newMessage;
        }

        private static string ReplaceInsertTags(string message, string insert, int index)
        {
            if (insert is null || !message.Contains($"[{DialogTextbox.insertTag}:{index}]"))
                return message;

            string newMessage = message.Replace($"[{DialogTextbox.insertTag}:{index}]", insert);
            return newMessage;
        }
        #endregion
    }
}