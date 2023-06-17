namespace BF2D.Game
{
    public class TcRunTextbox
    {
        public static void Run(string[] arguments)
        {
            if (!GameCtx.Instance.SystemTextbox.Armed)
            {
                Terminal.IO.LogWarning("The system textbox wasn't armed.");
                return;
            }

            GameCtx.Instance.SystemTextbox.TakeControl();
        }
    }
}