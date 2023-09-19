namespace BF2D.Game.Combat
{
    /// <summary>
    /// Add entries to this class's constructor to register terminal commands
    /// </summary>
    public class ShCombatRegistry : ShRegistry
    {
        public ShCombatRegistry()
        {
            this.commands = new()
            {
                //Add Commands Here
                //
                //
                //

                //{ "help", TcCombatHelp.Run },
                { "echo", SCEcho.Run },
                { "clear", SCClear.Run },
                { "paths", SCPaths.Run },
                { "combat", SCCombat.Run },
                { "combatdemo", SCCombatDemo.Run },
                { "message", SCMessage.Run },
                { "dialog", SCDialog.Run },
                { "dialogkey", SCDialogKey.Run },
                { "textbox", SCRunTextbox.Run },
                { "clearcaches", SCClearCaches.Run },
                { "savegame", SCSaveGame.Run },
                { "loadgame", SCLoadGame.Run },
                { "saveconfig", SCSaveControlsConfig.Run },
                { "loadconfig", SCLoadControlsConfig.Run },
                { "numrand", SCNumRand.Run },
                { "players", SCListPlayers.Run },
                { "embueitem", SCEmbueItem.Run },
                { "acquiregems", SCAcquireGems.Run },
                { "continue", SCContinue.Run }
            };
        }
    }
}