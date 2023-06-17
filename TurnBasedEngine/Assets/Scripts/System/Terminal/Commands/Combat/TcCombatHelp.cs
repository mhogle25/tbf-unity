namespace BF2D.Game.Combat
{
    public class TcCombatHelp
    {
        public static void Run(string[] arguments)
        {
            GameCtx.Instance.SystemTextbox.Dialog("help_combat", 0, null);
            TcRunTextbox.Run(null);
        }
    }
}