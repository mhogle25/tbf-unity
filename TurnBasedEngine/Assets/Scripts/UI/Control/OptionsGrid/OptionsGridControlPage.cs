using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BF2D.UI
{
    public class OptionsGridControlPage : OptionsGridControl
    {
        public int CurrentPage { get { return this.currentPage; } }
        public int PageCount
        {
            get
            {
                return (this.allOptions.Count + this.Controlled.Size - 1) /
                    this.Controlled.Size;
            }
        }
        public Enums.Axis PageOrientation { get { return this.pageOrientation; } set { this.pageOrientation = value; } }

        private readonly List<GridOption.Data> allOptions = new();
        private int currentPage = 0;

        [Header("Pages")]
        [SerializeField] private Enums.Axis pageOrientation = Enums.Axis.Horizontal;

        protected virtual void AddOption(GridOption.Data option)
        {
            this.allOptions.Add(option);
        }

        public virtual void LoadOptions(IEnumerable<GridOption.Data> options)
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

        public virtual void OnNavigate(OptionsGrid.Snapshot info)
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
                this.Controlled.SetCursorToLastElseFirst();
                return;
            }

            if (dimensionPrev == 0 && dimension == dimensionMax)
            {
                RefreshGrid(this.currentPage > 0 ? --this.currentPage : this.PageCount - 1);
                this.Controlled.SetCursorToLastElseFirst();
                return;
            }
        }

        protected void RefreshGrid(int index)
        {
            this.Controlled.Clear();

            if (this.allOptions.Count < 1)
                return;

            int startingIndex = this.Controlled.Size * index;
            int count = this.Controlled.Size;
            if (count > this.allOptions.Count - startingIndex)
                count = this.allOptions.Count - startingIndex;

            foreach (GridOption.Data data in this.allOptions.GetRange(startingIndex, count))
            {
                GridOption gridOption = this.Controlled.Add(data);
                gridOption.SetCursor(false);
            }

            if (!this.Controlled.Exists(this.Controlled.CursorPosition))
            {
                this.Controlled.CursorPosition = this.Controlled.LastOptionPosition;
            }
        }
    }
}