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
    }
}
