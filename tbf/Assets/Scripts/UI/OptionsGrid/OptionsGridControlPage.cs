using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D.UI
{
    public class OptionsGridControlPage : OptionsGridControl
    {
        public int CurrentPage => this.currentPage;
        public int PageCount => (this.allOptions.Count + this.Controlled.Area - 1) / this.Controlled.Area;
        public Enums.Axis PageOrientation { get => this.pageOrientation; set => this.pageOrientation = value; }

        public bool Armed => this.allOptions.Count > 0;

        protected readonly List<GridOption.Data> allOptions = new();
        private int currentPage = 0;

        [Header("Pages")]
        [SerializeField] private Enums.Axis pageOrientation = Enums.Axis.Horizontal;

        protected virtual void AddOption(GridOption.Data option)
        {
            this.allOptions.Add(option);
        }

        public void LoadOptions(IEnumerable<GridOption.Data> options)
        {
            ClearOptions();
            this.Controlled.Setup(this.Controlled.Width, this.Controlled.Height);

            foreach (GridOption.Data option in options)
                this.allOptions.Add(option);

            RefreshGrid(0);
        }

        public void ClearOptions()
        {
            this.allOptions.Clear();
            this.Controlled.Clear();
        }

        public void OnNavigate(OptionsGrid.NavInfo info)
        {
            if (this.allOptions.Count < 1)
                return;

            int dimensionPrev = info.cursorPositionPrev.x;
            int dimension = info.cursorPosition.x;
            int dimensionMax = this.Controlled.Width - 1;

            if (this.PageOrientation == Enums.Axis.Vertical)
            {
                dimensionPrev = info.cursorPositionPrev.y;
                dimension = info.cursorPosition.y;
                dimensionMax = this.Controlled.Height - 1;
            }

            if (dimensionPrev == dimensionMax && dimension == 0)
            {
                RefreshGrid(this.currentPage < this.PageCount - 1 ? ++this.currentPage : 0);
                this.Controlled.UtilityInitialize();
                return;
            }

            if (dimensionPrev == 0 && dimension == dimensionMax)
            {
                RefreshGrid(this.currentPage > 0 ? --this.currentPage : this.PageCount - 1);
                this.Controlled.UtilityInitialize();
                return;
            }
        }

        protected void RefreshGrid(int index)
        {
            this.Controlled.Clear();

            if (this.allOptions.Count < 1)
                return;

            int startingIndex = this.Controlled.Area * index;
            int count = this.Controlled.Area;
            if (count > this.allOptions.Count - startingIndex)
                count = this.allOptions.Count - startingIndex;

            foreach (GridOption.Data data in this.allOptions.GetRange(startingIndex, count))
            {
                GridOption gridOption = this.Controlled.Add(data);
                gridOption.SetCursor(false);
            }

            if (!this.Controlled.Exists(this.Controlled.CursorPosition))
                this.Controlled.SetCursorToLast();
        }
    }
}