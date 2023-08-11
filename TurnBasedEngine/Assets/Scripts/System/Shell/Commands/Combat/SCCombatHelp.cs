namespace BF2D.Game.Combat
{
    public class TcCombatHelp
    {
        public static void Run(string[] arguments)
        {
            UI.UICtx.One.SystemTextbox.Dialog("help_combat", 0);
            SCRunTextbox.Run(null);
        }
    }
}