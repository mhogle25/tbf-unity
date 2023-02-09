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

namespace BF2D
{
    public class TerminalInput : MonoBehaviour
    {
        private delegate void Command(string[] arguments);
        private readonly Dictionary<string, Command> commands;

        public TerminalInput()
        {
            this.commands = new()
            {
                { "echo", CommandEcho },
                { "clear", CommandClear },
                { "paths", CommandPaths },
                { "combatdemo", CommandCombatDemo }
            };
        }

        public void Commit(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

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
        #endregion
    }
}