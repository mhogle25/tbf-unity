using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;
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

        [SerializeField] private TerminalRegistry registry = null;

        private readonly Stack<string> historyBackward = new();
        private readonly Stack<string> historyForward = new();
        private bool inHistory = false;

        private void Awake()
        {
            this.historyBackward.Push("embueitem it_vorpalsword ch_vaporknight gm_burning_01 0 Brisingr");
            this.historyBackward.Push("combat save1 en_lessergoblin en_lessergoblin en_lessergoblin en_lessergoblin");
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

            if (!this.registry.ContainsKey(op))
            {
                Terminal.IO.LogWarning($"The command '{op}' does not exist.");
                return;
            }

            this.registry[op](args);
        }

        public void Commit(string op, string[] arguments)
        {
            if (!this.registry.ContainsKey(op))
            {
                Terminal.IO.LogWarning($"The operation '{op}' does not exist.");
                return;
            }

            this.registry[op](arguments);
        }

        #region Private Methods
        private string[] ParseCommand(string command)
        {
            string processed = command.Trim();

            //This regex splits the string by spaces, while ignoring spaces inside of parentheses
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
        #endregion
    }
}