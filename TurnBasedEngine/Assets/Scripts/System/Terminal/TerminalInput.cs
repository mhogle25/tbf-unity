using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;
using BF2D.Game;
using BF2D.Combat;
using BF2D.Enums;
using System.Text.RegularExpressions;
using System.Linq;
using BF2D.UI;

namespace BF2D
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TerminalInput : MonoBehaviour
    {
        private TMP_InputField inputField;

        private delegate void Command(string[] arguments);
        private readonly Dictionary<string, Command> commands;

        private readonly Stack<string> historyBackward = new();
        private readonly Stack<string> historyForward = new();
        private bool inHistory = false;

        public TerminalInput()
        {
            this.commands = new()
            {
                { "echo", CommandEcho },
                { "clear", CommandClear },
                { "paths", CommandPaths },
                { "combatdemo", CommandCombatDemo },
                { "message", CommandMessage },
            };
        }

        private void Awake()
        {
            this.inputField = GetComponent<TMP_InputField>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                UpKeyEvent();

            if (Input.GetKeyDown(KeyCode.DownArrow))
                DownKeyEvent();
        }

        public void Commit(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            ResetHistoryCursor();
            this.historyBackward.Push(command);
            string[] args = ParseCommand(command);

            if (args is null)
                return;

            string op = args[0];

            if (!this.commands.ContainsKey(op))
            {
                Terminal.IO.LogWarning($"The command '{op}' does not exist");
                return;
            }

            this.commands[op].Invoke(args);
        }

        private string[] ParseCommand(string command)
        {
            string processed = command.Trim();
            string[] regexed = Regex.Split(processed, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            List<string> args = new();
            for (int i = 0; i < regexed.Length; i++)
            {
                string element = regexed[i];
                if (string.IsNullOrEmpty(element))
                    continue;

                if (element.Contains('"') && i != 0)
                    element = element.Trim('"');

                args.Add(element);
            }

            return args.ToArray();
        }

        private void UpKeyEvent()
        {
            if (this.historyBackward.Count > 0)
            {
                string command = this.historyBackward.Pop();

                if (this.inHistory)
                    this.historyForward.Push(this.inputField.text);

                this.inHistory = true;

                this.inputField.text = command;
            }
        }

        private void DownKeyEvent()
        {
            if (this.historyForward.Count > 0)
            {
                string command = this.historyForward.Pop();
                this.historyBackward.Push(this.inputField.text);
                this.inputField.text = command;
            } 
            else
            {
                if (this.inHistory)
                {
                    this.historyBackward.Push(this.inputField.text);
                    this.inputField.text = string.Empty;
                }

                this.inHistory = false;
            }
        }

        private void ResetHistoryCursor()
        {
            if (this.inHistory)
            {
                this.inHistory = false;
                this.historyBackward.Push(this.inputField.text);
                while (this.historyForward.Count > 0)
                {
                    this.historyBackward.Push(this.historyForward.Pop());
                }
            }
        }

        #region Commands
        private void CommandEcho(string[] arguments)
        {
            if (arguments.Length < 2)
                return;

            for (int i = 1; i < arguments.Length; i++)
            {
                Terminal.IO.Log(arguments[i]);
            }
        }

        private void CommandClear(string[] arguments)
        {
            Terminal.IO.Clear();
        }

        private void CommandPaths(string[] arguments)
        {
            Terminal.IO.Log($"Streaming Assets Path: {Application.streamingAssetsPath}");
            Terminal.IO.Log($"Persistent Data Path: {Application.persistentDataPath}");
        }

        private void CommandCombatDemo(string[] arguments)
        {
            GameInfo.Instance.LoadControlsConfig(InputController.Keyboard, "default");
            GameInfo.Instance.LoadControlsConfig(InputController.Gamepad, "default");

            GameInfo.Instance.LoadGame("save1");
            List<CharacterStats> enemies = new()
            {
                GameInfo.Instance.InstantiateEnemy("en_lessergoblin")
            };

            GameInfo.Instance.StageCombatInfo(new CombatManager.InitializeInfo
            {
                players = GameInfo.Instance.Players,
                enemies = enemies
            });
        }

        private void CommandMessage(string[] arguments)
        {
            if (arguments.Length < 2)
            {
                Terminal.IO.LogWarning("Useage: message [text] (optional ->) [insert1] [insert2]...");
            }

            List<string> inserts = new();
            for (int i = 2; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }
            GameInfo.Instance.SystemTextbox.Textbox.Message(arguments[1], null, inserts);
            UIControlsManager.Instance.TakeControl(GameInfo.Instance.SystemTextbox);
        }
        #endregion
    }
}