using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace BF2D.Game.Actions
{
    public class StatsActionTargeter
    {
        public StatsAction StatsAction { set { this.statsAction = value; } }
        private StatsAction statsAction;

        public List<Stats> Targets { get { return this.targets; } set { this.targets = value; } }
        private List<Stats> targets = new List<Stats>();
    }
}
