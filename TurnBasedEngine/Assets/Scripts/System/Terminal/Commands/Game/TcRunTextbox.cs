namespace BF2D.Game
{
    public class TcRunTextbox
    {
        public static void Run(string[] arguments)
        {
            if (!GameCtx.Instance.SystemTextbox.Textbox.Armed)
            {
                Terminal.IO.LogWarning("The system textbox wasn't armed.");
                return;
            }

            if (UI.UIControlsManager.Instance.PhantomControlEnabled)
            {
                Terminal.IO.LogWarning("Can't run system textbox while another phantom UI control is active.");
                return;
            }

            UI.UIControlsManager.Instance.StartPhantomControl(GameCtx.Instance.SystemTextbox);
        }
    }
}