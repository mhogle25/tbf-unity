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
    public class TerminalRegistry
    {
        public delegate void Command(string[] arguments);
        private readonly Dictionary<string, Command> commands;

        public Command this[string key]
        {
            get
            {
                return this.commands[key];
            }
        }

        public bool ContainsKey(string key)
        {
            return this.commands.ContainsKey(key);
        }

        public TerminalRegistry()
        {
            this.commands = new()
            {
                { "help", CommandHelp },
                { "echo", CommandEcho },
                { "clear", CommandClear },
                { "paths", CommandPaths },
                { "combat", CommandCombat },
                { "combatdemo", CommandCombatDemo },
                { "message", CommandMessage },
                { "dialog", CommandDialog },
                { "dialogkey", CommandDialogKey },
                { "textbox", CommandRunTextbox },
                { "clearcaches", CommandClearCaches },
                { "savegame", CommandSaveGame },
                { "saveconfig", CommandSaveControlsConfig },
                { "loadconfig", CommandLoadControlsConfig }
            };
        }

        #region Commands
        private void CommandHelp(string[] arguments)
        {
            GameInfo.Instance.SystemTextbox.Textbox.Dialog("help_combat", 0, null);
            CommandRunTextbox(null);
        }

        private void CommandEcho(string[] arguments)
        {
            if (arguments.Length < 2)
                return;

            for (int i = 1; i < arguments.Length; i++)
            {
                Terminal.IO.LogQuiet(arguments[i]);
            }
        }

        private void CommandClear(string[] arguments)
        {
            Terminal.IO.Clear();
        }

        private void CommandPaths(string[] arguments)
        {
            Terminal.IO.LogQuiet($"Streaming Assets Path: {Application.streamingAssetsPath}");
            Terminal.IO.LogQuiet($"Persistent Data Path: {Application.persistentDataPath}");
        }

        private void CommandCombat(string[] arguments)
        {
            //
            // Delete this
            //
            GameInfo.Instance.LoadControlsConfig(InputController.Keyboard, BF2D.Game.Strings.System.Default);
            GameInfo.Instance.LoadControlsConfig(InputController.Gamepad, BF2D.Game.Strings.System.Default);
            //
            //
            //

            const string warning = "Useage: combat [saveFileID] [enemyID1] (optional ->) [enemyID2]...";

            if (arguments.Length < 3)
            {
                Terminal.IO.LogWarningQuiet(warning);
                return;
            }

            string saveFileID = arguments[1];

            GameInfo.Instance.LoadGame(saveFileID);
            if (!GameInfo.Instance.SaveActive)
            {
                Terminal.IO.LogErrorQuiet("Save file load failed.");
                return;
            }

            List<CharacterStats> enemies = new();
            for (int i = 2; i < arguments.Length; i++)
            {
                CharacterStats enemy = GameInfo.Instance.InstantiateEnemy(arguments[i]);
                if (enemy is null)
                {
                    Terminal.IO.LogErrorQuiet($"Enemy with ID {arguments[i]} does not exist.");
                    return;
                }
                enemies.Add(enemy);
            }

            CombatManager.Instance.CancelCombat();

            GameInfo.Instance.StageCombatInfo(new CombatManager.InitializeInfo
            {
                players = GameInfo.Instance.ActivePlayers,
                enemies = enemies
            });
        }

        private void CommandCombatDemo(string[] arguments)
        {
            CommandCombat(new string[] { "combat", "save1", "en_lessergoblin" });
        }

        private void CommandMessage(string[] arguments)
        {
            if (arguments.Length < 2)
            {
                Terminal.IO.LogWarningQuiet("Useage: message [text] (optional ->) [insert1] [insert2]...");
                return;
            }

            List<string> inserts = new();
            for (int i = 2; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            GameInfo.Instance.SystemTextbox.Textbox.Message(arguments[1], null, inserts);
            Terminal.IO.LogQuiet("Pushed a message to the system textbox's queue. Run with 'textbox'.");
        }

        private void CommandDialog(string[] arguments)
        {
            const string warningMessage = "Useage: dialog [startingIndex] [length] [line1] [line2]... (optional ->) [insert1] [insert2]...";

            if (arguments.Length < 4)
            {
                Terminal.IO.LogWarningQuiet(warningMessage);
                return;
            }

            int startingIndex;
            try
            {
                startingIndex = int.Parse(arguments[1]);
            }
            catch
            {
                Terminal.IO.LogWarningQuiet(warningMessage);
                return;
            }

            int length;
            try
            {
                length = int.Parse(arguments[2]);
            }
            catch
            {
                Terminal.IO.LogWarningQuiet(warningMessage);
                return;
            }

            if (length > arguments.Length - 3)
            {
                Terminal.IO.LogWarningQuiet("Length was greater than the number of lines. " + warningMessage);
                return;
            }

            if (startingIndex >= length)
            {
                Terminal.IO.LogWarningQuiet("Starting index was outside the range of the dialog. " + warningMessage);
                return;
            }

            List<string> dialog = new();
            for (int i = 3; i < length + 3; i++)
            {
                dialog.Add(arguments[i]);
            }

            List<string> inserts = new();
            for (int i = length + 3; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            GameInfo.Instance.SystemTextbox.Textbox.Dialog(dialog, startingIndex, null, inserts);
            Terminal.IO.LogQuiet("Pushed a dialog to the system textbox's queue. Run with 'textbox'.");
        }

        private void CommandDialogKey(string[] arguments)
        {
            const string warningMessage = "Useage: dialogkey [startingIndex] [key] (optional ->) [insert1] [insert2]...";

            if (arguments.Length < 3)
            {
                Terminal.IO.LogWarningQuiet(warningMessage);
                return;
            }

            int startingIndex;
            try
            {
                startingIndex = int.Parse(arguments[1]);
            }
            catch
            {
                Terminal.IO.LogWarningQuiet(warningMessage);
                return;
            }

            List<string> inserts = new();
            for (int i = 3; i < arguments.Length; i++)
            {
                inserts.Add(arguments[i]);
            }

            GameInfo.Instance.SystemTextbox.Textbox.Dialog(arguments[2], startingIndex, null, inserts);
            Terminal.IO.LogQuiet("Pushed a dialog to the system textbox's queue. Run with 'textbox'.");
        }

        private void CommandRunTextbox(string[] arguments)
        {
            if (!GameInfo.Instance.SystemTextbox.Textbox.Armed)
            {
                Terminal.IO.LogWarningQuiet("The system textbox wasn't armed.");
                return;
            }

            if (UIControlsManager.Instance.PhantomControlEnabled)
            {
                Terminal.IO.LogWarningQuiet("Can't run system textbox while another phantom UI control is active.");
                return;
            }

            UIControlsManager.Instance.StartPhantomControl(GameInfo.Instance.SystemTextbox);
        }

        private void CommandClearCaches(string[] arguments)
        {
            GameInfo.Instance.ClearCaches();
        }

        private void CommandSaveGame(string[] arguments)
        {
            if (arguments.Length == 1)
            {
                GameInfo.Instance.SaveGame();
                return;
            }

            for (int i = 1; i < arguments.Length; i++)
            {
                string newSaveFileID = arguments[i];
                GameInfo.Instance.SaveGameAs(newSaveFileID);
            }
        }

        private void CommandSaveControlsConfig(string[] arguments)
        {
            const string warningMessage = "Useage: saveconfig [keyboard OR gamepad] (optional ->) [fileID1] [fileID2]...";

            if (arguments.Length < 2)
            {
                Terminal.IO.LogWarning(warningMessage);
                return;
            }

            string type = arguments[1];
            type = type.ToLower();
            type = type.Trim();
            InputController controllerType;
            if (type == "keyboard")
                controllerType = InputController.Keyboard;
            else if (type == "gamepad")
                controllerType = InputController.Gamepad;
            else
            {
                Terminal.IO.LogWarning($"Unknown controller type '{arguments[1]}'. " + warningMessage);
                return;
            }

            if (arguments.Length == 2)
            {
                GameInfo.Instance.SaveControlsConfig(controllerType);
                return;
            }

            for (int i = 2; i < arguments.Length; i++)
            {
                string newControlsConfigID = arguments[i];
                GameInfo.Instance.SaveControlsConfigAs(controllerType, newControlsConfigID);
            }
        }

        private void CommandLoadControlsConfig(string[] arguments)
        {
            const string warningMessage = "Useage: loadconfig [keyboard OR gamepad] [fileID]";

            if (arguments.Length < 3 || arguments.Length > 3)
            {
                Terminal.IO.LogWarning(warningMessage);
                return;
            }

            string type = arguments[1];
            InputController controllerType;
            if (type == "keyboard")
                controllerType = InputController.Keyboard;
            else if (type == "gamepad")
                controllerType = InputController.Gamepad;
            else
            {
                Terminal.IO.LogWarning($"Unknown controller type '{arguments[1]}'. " + warningMessage);
                return;
            }

            GameInfo.Instance.LoadControlsConfig(controllerType, arguments[2]);
        }
        #endregion
    }

}