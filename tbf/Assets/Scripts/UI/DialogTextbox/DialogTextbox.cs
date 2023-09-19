using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Newtonsoft.Json;
using BF2D.Enums;

namespace BF2D.UI
{
    public class DialogTextbox : UIUtility
    {
        private struct Data
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
        public float defaultMessageSpeed = 0.05f;
        public bool messageInterrupt = false;
        public bool autoPass = false;

        public UnityEvent OnEndOfQueuedDialogs => this.onEndOfQueuedDialogs;
        [SerializeField] private UnityEvent onEndOfQueuedDialogs = new();

        [Header("Dialog Responses")]
        [SerializeField] private Utilities.FileManager responsesFileManager = null;
        [SerializeField] private OptionsGridControl responseOptionsControl = null;
        [SerializeField] private GameCondition prereqConditionChecker = null;
        
        [Serializable] public class ResponseOptionEvent : UnityEvent<string> { }
        public ResponseOptionEvent ResponseConfirmEvent => this.responseOptionConfirmEvent;
        [SerializeField] private ResponseOptionEvent responseOptionConfirmEvent = new();

        private readonly List<IResponseController> responseControllers = new();

        public UnityEvent ResponseBackEvent => this.responseOptionBackEvent;
        [SerializeField] private UnityEvent responseOptionBackEvent = new();

        public bool ResponseBackEventEnabled
        {
            get => this.responseOptionsControl.Controlled.BackEnabled;
            set => this.responseOptionsControl.Controlled.BackEnabled = value;
        }

        [Header("Audio")]
        [SerializeField] private AudioClipCollection voiceCollection = null;
        [SerializeField] private AudioSource confirmAudioSource = null;
        [SerializeField] private AudioSource voiceAudioSource = null;
        [SerializeField] private AudioClip defaultVoice = null;
        [Header("Assets")]
        //File Manager
        //Armed dialogs
        private Dictionary<string, List<string>> dialogs = new();
        //Armed dialog options
        private Dictionary<string, string> dialogResponses = new();

        //The state delegate
        private Action state = null;

        public bool Armed => this.dialogQueue.Count > 0;
        //The dialog queue
        private readonly Queue<Data> dialogQueue = new();

        //The current dialog data
        private Data currentDialog = default;

        //Control Variables
        private float timeAccumulator = 0f;
        private float messageSpeed = 0f;
        private int dialogIndex = 0;
        private int messageIndex = 0;
        private bool pass = false;
        private int nextDialogIndex = -1;
        private bool continueFlag = false;

        //Misc
        private const int DEFAULT_VALUE = -1;
        private const char PAUSE_TAG = 'P';
        private const char SPEED_TAG = 'S';
        private const char VOICE_TAG = 'V';
        private const char JUMP_TAG = 'J';
        private const char NAME_TAG = 'N';
        private const char END_TAG = 'E';
        private const char RESPONSE_TAG = 'R';
        private const char INSERT_TAG = 'I';

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
            this.View.gameObject.SetActive(true);
            this.state = StateDialogQueue;
            this.textField.text = string.Empty;
            base.UtilityInitialize();
        }

        public override void UtilityFinalize()
        {
            NametagDisable();
            Cancel();
            base.UtilityFinalize();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Pushes a single message to the dialog queue with a callback function
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <param name="callback">Called at the end of dialog</param>
        /// <param name="inserts">Will replace insert tags in dialog by index</param>
        public void Message(string message = null, Action callback = null, params string[] inserts)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[DialogTextbox] Tried to queue a message but the message was null or empty");
                return;
            }

            List<string> lines = new()
            {
                AppendEndTag(message)
            };

            lines = ReplaceInsertTags(lines, inserts);

            Data dialogData = new()
            {
                dialog = lines,
                index = 0,
                callback = callback
            };

            this.dialogQueue.Enqueue(dialogData);
        }

        /// <summary>
        /// Pushes a dialog from the list of loaded dialog files to the dialog queue with a callback function
        /// </summary>
        /// <param name="id">The filename of the desired dialog</param>
        /// <param name="startingLine">The line the dialog will start from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        /// <param name="inserts">Will replace insert tags in dialog by index</param>
        public void Dialog(string id = "", int startingLine = 0, Action callback = null, params string[] inserts)
        {
            if (!this.dialogs.ContainsKey(id))
            {
                Debug.LogError($"[DialogTextbox] The key '{id}' was not found in the dialogs dictionary");
                return;
            }

            List<string> lines = this.dialogs[id];

            if (startingLine < 0 || startingLine >= lines.Count)
            {
                Debug.LogError("[DialogTextbox:Dialog] Tried to queue a dialog but the starting line index was outside the range of the dialog");
                return;
            }

            lines = ReplaceInsertTags(lines, inserts);

            Data dialogData = new()
            {
                dialog = lines,
                index = startingLine,
                callback = callback,
            };

            this.dialogQueue.Enqueue(dialogData);
        }

        /// <summary>
        /// Pushes a dialog to the dialog queue
        /// </summary>
        /// <param name="lines">The dialog to be displayed</param>
        /// <param name="startingLine">The line the dialog starts from (0 is the first line)</param>
        /// <param name="callback">Called at the end of dialog</param>
        /// <param name="inserts">Will replace insert tags in dialog by index</param>
        public void Dialog(List<string> lines = null, int startingLine = 0, Action callback = null, params string[] inserts)
        {
            if (lines is null || lines.Count < 1)
            {
                Debug.LogError("[DialogTextbox:Dialog] Tried to queue a dialog but the dialog was null or empty");
                return;
            }

            if (startingLine < 0 || startingLine >= lines.Count)
            {
                Debug.LogError("[DialogTextbox:Dialog] Tried to queue a dialog but the starting line index was outside the range of the dialog");
                return;
            }

            List<string> newLines = ReplaceInsertTags(lines, inserts);
            Data dialogData = new()
            {
                dialog = newLines,
                index = startingLine,
                callback = callback,
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

            if (this.state == StateMessageParseAndDisplay && messageInterrupt) // If the confirm button is pressed and interrupt is on, switch to instantaneous parse
            {
                MessageParseAndDisplayInstantaneous();
            }
            else if (this.state == StateEndOfLine || this.state == StateEndOfDialog)
            {
                this.continueFlag = true;
            }
        }

        public void Cancel()
        {
            this.dialogQueue.Clear();
            this.state = StateDialogQueue;
        }

        public void AddResponseController(IResponseController controller)
        {
            if (controller is null)
            {
                Debug.Log("[DialogTextbox.AddResponseController] The controller was null");
                return;
            }

            this.responseControllers.Add(controller);
        }

        public void RemoveResponseController(IResponseController controller)
        {
            if (controller is null)
            {
                Debug.Log("[DialogTextbox.RemoveResponseController] The controller was null");
                return;
            }

            if (!this.responseControllers.Contains(controller))
            {
                Debug.Log($"[DialogTextbox.RemoveResponseController] The controller is not one of {this.name}'s response controllers");
                return;
            }

            this.responseControllers.Remove(controller);
        }

        public void ClearResponseControllers() => this.responseControllers.Clear();
        #endregion

        #region Public Static Utilities
        public static List<string> ReplaceInsertTags(List<string> dialog, string[] inserts)
        {
            if (inserts is null || inserts.Length < 1)
                return dialog;

            List<string> newDialog = new();
            for (int i = 0; i < dialog.Count; i++)
                newDialog.Add(ReplaceInsertTags(dialog[i], inserts));

            return newDialog;
        }

        public static List<string> AppendDelayToEachLine(List<string> dialog, float delaySecs) =>
            dialog.Select(line => $"{line}[{DialogTextbox.PAUSE_TAG}:{delaySecs}]").ToList();
        

        public static string AppendEndTag(string line) =>
            $"{line}[{DialogTextbox.END_TAG}]";

        public static List<string> AppendEndTag(List<string> dialog)
        {
            List<string> newDialog = new();
            int count = dialog.Count;
            for (int i = 0; i < count; i++)
            {
                string line = dialog[i];
                if (i == count - 1)
                    line = AppendEndTag(line);
                newDialog.Add(line);
            }

            return newDialog;
        }
        #endregion

        #region States
        private void StateDialogQueue()
        {
            if (this.dialogQueue.Count < 1)
                return;

            Data dialogData = this.dialogQueue.Dequeue();

            ResetControlVariables(dialogData.index);
            this.voiceAudioSource.clip = this.defaultVoice;
            this.currentDialog = dialogData;
            this.textField.text = "";

            //Debug.Log("[DialogTextbox] Dialog Armed\n" + this.activeLines.Count + " lines");

            this.state = StateMessageParseAndDisplay;
        }

        private void StateMessageParseAndDisplay() {
            //Message Parse Statement
            if (Time.time > this.timeAccumulator) {
                this.timeAccumulator = Time.time + this.messageSpeed;       //Time increment
                MessageParseAndDisplay();                                   //Call the message parser and display the next character or execute the next flag
            }
        }

        private void StateEndOfLine() {
            if (!this.continueIcon.enabled)
                this.continueIcon.enabled = true;

            if (this.autoPass)
                Continue();

            if (this.continueFlag || this.pass) {
                this.continueFlag = false;
                this.pass = false;
                Utilities.Audio.PlayAudioSource(this.confirmAudioSource);       //Play the confirm sound
                this.continueIcon.enabled = false;
                this.textField.text = "";
                if (this.nextDialogIndex != DialogTextbox.DEFAULT_VALUE)
                {
                    this.dialogIndex = this.nextDialogIndex;
                    this.nextDialogIndex = DialogTextbox.DEFAULT_VALUE;
                } 
                else
                {
                    this.dialogIndex++;                     //Increment dialog index to the next line of dialog
                }
                this.messageIndex = 0;                      //Reset the message index to be on the first character of the line
                this.state = StateMessageParseAndDisplay;   //Change the state to MessageParseAndDisplay
            }
        }

        private void StateEndOfDialog() {
            if (this.nextDialogIndex != DialogTextbox.DEFAULT_VALUE)
                this.state = StateEndOfLine;

            if (!this.continueIcon.enabled)
                this.continueIcon.enabled = true;

            if (this.autoPass)
                Continue();

            if (this.continueFlag || this.pass) {
                this.pass = false;
                Utilities.Audio.PlayAudioSource(this.confirmAudioSource);   //Play the confirm sound
                this.continueIcon.enabled = false;
                NametagDisable();
                //Call the callback function if it exists
                this.currentDialog.callback?.Invoke();
                //Call the EOD event
                if (this.dialogQueue.Count < 1)
                    this.onEndOfQueuedDialogs?.Invoke();
                //Reset the State
                this.state = StateDialogQueue;
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
            this.messageSpeed = this.defaultMessageSpeed;
            this.pass = false;
            this.nextDialogIndex = -1;
            this.continueFlag = false;
        }

        private void MessageParseAndDisplayInstantaneous() {
            while (MessageParseAndDisplay());   //Setup parse and display until end of line, end of dialog, or option response is called
            this.timeAccumulator = 0f;
        }

        private bool MessageParseAndDisplay() {
            if (this.dialogIndex >= this.currentDialog.dialog.Count)
            {
                Debug.LogError($"[DialogTextbox:MessageParseAndDisplay] Tried to parse but the dialog index was out of range. Did you forget to use an end tag? (Previous line: {this.currentDialog.dialog[this.dialogIndex - 1]})");
                Cancel();
                return true;
            }

            string message = this.currentDialog.dialog[this.dialogIndex];    //Set message to the current line of dialog

            //If our message index is greater than the length of the message
            if (message.Length <= this.messageIndex) {
                //Change the state to Eol
                this.state = StateEndOfLine;

                return false;
            }

            //Begin tag parsing
            if (message[this.messageIndex] == '[')
            {
                //Take and read tag
                char op = message[this.messageIndex + 1];
                int newMessageIndex = this.messageIndex;
                switch (op)
                {
                    case DialogTextbox.PAUSE_TAG: PauseAction(message, ref newMessageIndex); break;   //Case: Pause for seconds
                    case DialogTextbox.SPEED_TAG: SpeedAction(message, ref newMessageIndex); break;   //Case: New text speed
                    case DialogTextbox.NAME_TAG: NametagAction(message, ref newMessageIndex); break;  //Case: Orator name
                    case DialogTextbox.JUMP_TAG: JumpAction(message, ref newMessageIndex); break;     //Case: Jump
                    case DialogTextbox.VOICE_TAG: VoiceAction(message, ref newMessageIndex); break;   //Case: Voice
                    case DialogTextbox.RESPONSE_TAG: ResponseAction(message, ref newMessageIndex); return false;
                    case DialogTextbox.END_TAG: this.state = StateEndOfDialog; return false;
                    default:
                        if (op == DialogTextbox.INSERT_TAG)
                            Debug.LogError($"[DialogTextbox:MessageParseAndDisplay] Message '{message}' has incorrectly formatted insert tags");
                        else
                            Debug.LogError($"[DialogTextbox:MessageParseAndDisplay] Tag '{op}' was not valid");
                        Cancel();
                        return true;
                }
            }
            else
            { //Basic character
                if (message[this.messageIndex] != ' ')
                    BF2D.Utilities.Audio.PlayAudioSource(this.voiceAudioSource);

                string currentMessage = this.textField.text;
                currentMessage += message[this.messageIndex];
                this.textField.text = currentMessage;

                this.messageIndex++;                                                                    //Increment message index to move to next character
            }

            return true;
        }

        private void PauseAction(string message, ref int newMessageIndex)
        {
            float wait = float.Parse(ParseTag(message, ref newMessageIndex));           //Add a pause to the time accumulator
            this.timeAccumulator += wait;
            this.messageIndex = newMessageIndex + 1;                                    //Increment the message index accordingly
        }

        private void SpeedAction(string message, ref int newMessageIndex)
        {
            float newSpeed = float.Parse(ParseTag(message, ref newMessageIndex));
            newSpeed = newSpeed >= 0 ? newSpeed : defaultMessageSpeed;                  //If the new speed is less than 0, set it to the default speed
            this.messageSpeed = newSpeed;
            this.messageIndex = newMessageIndex + 1;                                    //Increment the message index accordingly
        }

        private void NametagAction(string message, ref int newMessageIndex)
        {
            string name = ParseTag(message, ref newMessageIndex);
            if (name == DialogTextbox.DEFAULT_VALUE.ToString())
                NametagDisable();
            else
                NametagEnable(name);
            this.messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
        }

        private void JumpAction(string message, ref int newMessageIndex)
        {
            int newDialogIndex = int.Parse(ParseTag(message, ref newMessageIndex));
            this.dialogIndex = newDialogIndex;
            this.messageIndex = 0;
        }

        private void VoiceAction(string message, ref int newMessageIndex)
        {
            string key = ParseTag(message, ref newMessageIndex);

            if (this.voiceCollection.Contains(key))
            {
                this.voiceAudioSource.clip = this.voiceCollection.Get(key);
            }
            else if (key == DialogTextbox.DEFAULT_VALUE.ToString())
            {
                this.voiceAudioSource.clip = this.defaultVoice;
            }
            else
            {
                Debug.LogError($"[DialogTextbox:MessageParseAndDisplay] Voice key '{key}' was not found in the voices dictionary");
            }
            this.messageIndex = newMessageIndex + 1;                                        //Increment the message index accordingly
        }

        private void ResponseAction(string message, ref int newMessageIndex)
        {
            string data = ParseTag(message, ref newMessageIndex);

            if (string.IsNullOrEmpty(data))
            {
                Debug.LogError("[DialogTextbox:MessageParseAndDisplay] The value for the response data cannot be null");
                Cancel();
                return;
            }

            List<ResponseData> options;

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
                    Debug.LogError($"[DialogTextbox:MessageParseAndDisplay] The dialog response file for the specified key '{data}' was not found");
                    Cancel();
                    return;
                }
            }

            SetupResponses(options);
            this.messageIndex = newMessageIndex + 1;
        }

        private string ParseTag(string message, ref int index) {
            //Move to tag
            index++;
            //Move to colon, check if colon exists, move on
            index++;
            if (message[index] != ':')
            {
                Debug.LogError("[DialogTextbox:ParseTag] Incorrect Syntax, add ':' after tag");
            }
            index++;

            char character = message[index];                //Initialize character index
            string valueString = string.Empty;              //Float before conversion

            Stack<char> stack = new();
            while (character != ']' || stack.Count > 0)
            {
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

        private void NametagEnable(string name)
        {
            this.nametag.gameObject.SetActive(true);
            this.nametagTextField.text = name;
        }

        private void NametagDisable()
        {
            this.nametag.gameObject.SetActive(false);
        }

        private static bool ValidJson(string json)
        {
            try
            {
                JsonConvert.DeserializeObject<object>(json, new Newtonsoft.Json.Converters.StringEnumConverter());
            } 
            catch
            {
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
                Debug.LogError(x.Message);
                return new List<ResponseData>();
            }
            return responseData;
        }

        private void SetupResponses(List<ResponseData> options)
        {
            this.responseOptionsControl.Controlled.Setup(1, options.Count);

            foreach (ResponseData option in options)
            {
                if (this.prereqConditionChecker && this.prereqConditionChecker.CheckCondition(option.prereq.ToString()))
                    continue;

                string action = option.action.ToString();

                this.responseOptionsControl.Controlled.Add(new GridOption.Data
                {
                    text = option.text,
                    onInput = new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = () =>
                        {
                            foreach (IResponseController controller in this.responseControllers)
                                controller.OnConfirm(action);

                            this.responseOptionConfirmEvent?.Invoke(action);

                            ResponseConfirm(option.dialogIndex);
                        },
                        [InputButton.Back] = () =>
                        {
                            FinalizeResponse();

                            foreach (IResponseController controller in this.responseControllers)
                                controller.OnBack();

                            this.responseOptionBackEvent?.Invoke();
                        }
                    }
                });
            }

            UICtx.StartControlGeneric(this.responseOptionsControl);
            this.responseOptionsControl.Controlled.SetCursorToFirst();
        }

        private void ResponseConfirm(int dialogIndex)
        {
            if (dialogIndex != DialogTextbox.DEFAULT_VALUE)
                this.nextDialogIndex = dialogIndex;

            FinalizeResponse();
            this.pass = true;
            this.state = StateMessageParseAndDisplay;
        }

        private void FinalizeResponse()
        {
            UICtx.EndControlGeneric(this.responseOptionsControl);
            this.responseOptionsControl.gameObject.SetActive(false);
        }
        #endregion

        #region Private Static Utilities
        private static string ReplaceInsertTags(string message, string[] inserts)
        {
            if (inserts is null)
                return message;
            if (inserts.Length < 1)
                return message;

            string newMessage = message;
            for (int i = 0; i < inserts.Length; i++)
                newMessage = ReplaceInsertTags(newMessage, inserts[i], i);

            return newMessage;
        }

        private static string ReplaceInsertTags(string message, string insert, int index)
        {
            if (insert is null || !message.Contains($"[{DialogTextbox.INSERT_TAG}:{index}]"))
                return message;

            string newMessage = message.Replace($"[{DialogTextbox.INSERT_TAG}:{index}]", insert);
            return newMessage;
        }

        #endregion
    }
}