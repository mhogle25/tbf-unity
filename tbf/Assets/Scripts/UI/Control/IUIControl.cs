using UnityEngine;

namespace BF2D.UI
{
    public abstract class UIControl : MonoBehaviour
    {
        public abstract void ControlInitialize();
        public abstract void ControlFinalize();
    }
}
