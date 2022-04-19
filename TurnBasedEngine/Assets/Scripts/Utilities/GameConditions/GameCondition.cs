using UnityEngine;

namespace BF2D
{
    public class GameCondition : MonoBehaviour
    {
        public virtual bool CheckCondition(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;

            return true;
        }
    }

}
