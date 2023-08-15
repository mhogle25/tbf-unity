using UnityEngine;
using System.Collections.Generic;

namespace BF2D.UI
{
    public class OptionsGridControlInit : OptionsGridControl
    {
        [Header("Init")]
        [SerializeField] private List<GridOption> initGridOptions = new();

        protected override void Awake()
        {
            base.Awake();
            GridInitialize();
        }

        public void GridInitialize()
        {
            if (!this.Controlled.Initialized)
                this.Controlled.LoadOptions(this.initGridOptions);
        }
    }
}
