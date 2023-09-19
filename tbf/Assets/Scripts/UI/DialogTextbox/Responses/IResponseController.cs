namespace BF2D.UI
{
    public interface IResponseController
    {
        public void OnConfirm(string json);
        public void OnBack();
    }
}