using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading;
using System;

namespace BF2D.UI
{
    public abstract class UIControl : UIComponent
    {
        public abstract void ControlInitialize();
        public abstract void ControlFinalize();
    }
}
