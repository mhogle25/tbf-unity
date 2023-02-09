using BF2D.UI;
using UnityEngine;

namespace BF2D.Game
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public void PassControlBack()
        {
            UIControlsManager.Instance.PassControlBack();
        }
    }
}
