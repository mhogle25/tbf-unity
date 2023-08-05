namespace BF2D.Game
{
    public class SCRunTextbox
    {
        public static void Run(params string[] arguments)
        {
            if (!UI.UICtx.One.SystemTextbox.Armed)
            {
                ShCtx.One.LogWarning("The system textbox wasn't armed.");
                return;
            }

            UI.UICtx.One.SystemTextbox.TakeControl();
        }
    }
}