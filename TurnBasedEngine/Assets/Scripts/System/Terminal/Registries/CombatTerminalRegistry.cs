using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;
using BF2D.Enums;
using System.Text.RegularExpressions;
using System.Linq;
using BF2D.UI;

namespace BF2D.Game.Combat
{
    /// <summary>
    /// Add entries to this class's constructor to register terminal commands
    /// </summary>
    public class CombatTerminalRegistry : TerminalRegistry
    {
        public CombatTerminalRegistry()
        {
            this.commands = new()
            {
                //Add Commands Here
                //
                //
                //

                { "help", TcCombatHelp.Run },
                { "echo", TcEcho.Run },
                { "clear", TcClear.Run },
                { "paths", TcPaths.Run },
                { "combat", TcCombat.Run },
                { "combatdemo", TcCombatDemo.Run },
                { "message", TcMessage.Run },
                { "dialog", TcDialog.Run },
                { "dialogkey", TcDialogKey.Run },
                { "textbox", TcRunTextbox.Run },
                { "clearcaches", TcClearCaches.Run },
                { "savegame", TcSaveGame.Run },
                { "saveconfig", TcSaveControlsConfig.Run },
                { "loadconfig", TcLoadControlsConfig.Run },
            };
        }
    }

}