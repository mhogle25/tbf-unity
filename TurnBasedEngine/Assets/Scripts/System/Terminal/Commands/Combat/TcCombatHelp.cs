namespace BF2D.Game.Combat
{
    public class TcCombatHelp
    {
        public static void Run(string[] arguments)
        {
            GameCtx.Instance.SystemTextbox.Textbox.Dialog("help_combat", false, 0, null);
            TcRunTextbox.Run(null);
        }
    }
}