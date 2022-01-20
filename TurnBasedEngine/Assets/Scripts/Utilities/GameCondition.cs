using UnityEngine;

namespace BF2D
{
    public class GameCondition : MonoBehaviour
    {
        public bool CheckJsonCondition(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            return true;
        }
    }

}
