using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D.UI
{
    public abstract class OptionsGridControlPage : OptionsGridControl
    {
        private List<GridOption.Data> allOptions = new();
        private int currentIndex = 0;
        private int PageCount
        {
            get
            {
                return (this.allOptions.Count + this.controlledOptionsGrid.Size - 1) /
                    this.controlledOptionsGrid.Size;
            }
        }

        public Enums.Axis PageOrientation { get { return this.pageOrientation; } set { this.pageOrientation = value; } }
        [Header("Pages")]
        [SerializeField] private Enums.Axis pageOrientation = Enums.Axis.Horizontal;

        public virtual void AddOption(GridOption.Data option)
        {
            this.allOptions.Add(option);
        }

        public virtual void LoadOptions(IEnumerable<GridOption.Data> options)
        {
            ClearOptions();
            foreach (GridOption.Data option in options)
                this.allOptions.Add(option);

            RefreshGrid(0);
        }

        public virtual void ClearOptions()
        {
            this.allOptions.Clear();
            this.controlledOptionsGrid.Clear();
        }

        public virtual void OnNavigate(OptionsGrid.NavigateInfo info)
        {
            if (this.allOptions.Count < 1)
                return;

            if (this.PageCount < 2)
                return;

            int dimensionPrev = info.cursorPositionPrev.x;
            int dimension = info.cursorPosition.x;
            int dimensionMax = this.controlledOptionsGrid.Width - 1;

            if (this.PageOrientation == Enums.Axis.Vertical)
            {
                dimensionPrev = info.cursorPositionPrev.y;
                dimension = info.cursorPosition.y;
                dimensionMax = this.controlledOptionsGrid.Height - 1;
            }

            if (dimensionPrev == dimensionMax && dimension == 0)
                RefreshGrid(this.currentIndex < this.PageCount - 1 ? ++this.currentIndex : 0);

            if (dimensionPrev == 0 && dimension == dimensionMax)
                RefreshGrid(this.currentIndex > 0 ? --this.currentIndex : this.PageCount - 1);
        }

        protected void RefreshGrid(int index)
        {
            this.controlledOptionsGrid.Clear();

            if (this.allOptions.Count < 1)
                return;

            int startingIndex = this.controlledOptionsGrid.Size * index;
            int count = this.controlledOptionsGrid.Size;
            if (count > this.allOptions.Count - startingIndex)
                count = this.allOptions.Count - startingIndex;

            foreach (GridOption.Data data in
                this.allOptions.GetRange(startingIndex, count))
            {
                this.controlledOptionsGrid.Add(data);
            }
        }
    }
}