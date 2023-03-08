namespace BF2D.Game
{
    public class TcRunTextbox
    {
        public static void Run(string[] arguments)
        {
            if (!GameInfo.Instance.SystemTextbox.Textbox.Armed)
            {
                Terminal.IO.LogWarningQuiet("The system textbox wasn't armed.");
                return;
            }

            if (UI.UIControlsManager.Instance.PhantomControlEnabled)
            {
                Terminal.IO.LogWarningQuiet("Can't run system textbox while another phantom UI control is active.");
                return;
            }

            UI.UIControlsManager.Instance.StartPhantomControl(GameInfo.Instance.SystemTextbox);
        }
    }
}