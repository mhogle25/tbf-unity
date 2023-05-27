using System;
using UnityEngine;
using BF2D.Enums;
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
            LoadOptionsIntoGrid(this.controlled, this.initGridOptions);
        }

        protected void LoadOptionsIntoGrid(OptionsGrid grid, List<GridOption> initGridOptions)
        {
            if (grid.Width > 0 && grid.Height > 0)
                grid.Setup(grid.Width, grid.Height);

            if (initGridOptions.Count > 0)
                foreach (GridOption option in initGridOptions)
                    grid.Add(option);
        }
    }
}
