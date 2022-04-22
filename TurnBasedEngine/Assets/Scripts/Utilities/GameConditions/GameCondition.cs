using UnityEngine;

namespace BF2D
{
    public abstract class GameCondition : MonoBehaviour
    {
        public abstract bool CheckCondition(string json);
    }

}
