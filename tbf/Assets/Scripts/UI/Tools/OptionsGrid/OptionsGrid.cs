using System.Collections.Generic;
using UnityEngine;
using System;
using BF2D.Enums;
using BF2D.Attributes;
using UnityEngine.Events;

namespace BF2D.UI
{
    public class OptionsGrid : UIUtility
    {
        [Header("Grid")]
        [SerializeField] private GridOption optionPrefab = null;
        [Tooltip("The container for grid options")]
        [SerializeField] protected Transform container = null;
        [Tooltip("Determines the direction that elements will be populated in")]
        [SerializeField] private Axis instantiationAxis = Axis.Horizontal;
        [SerializeField] protected int gridWidth = 1;
        [SerializeField] protected int gridHeight = 1;

        public struct NavInfo
        {
            public int cursorPosition1D;
            public Vector2Int cursorPosition;
            public int cursorPosition1DPrev;
            public Vector2Int cursorPositionPrev;
        }

        [SerializeField] private UnityEvent<NavInfo> onNavigate = new();

        [Header("Audio")]
        [SerializeField] private AudioSource navigateAudioSource = null;
        [Tooltip("Enable/disable use of the confirm button with the grid")]
        [SerializeField] private bool confirmEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(confirmEnabled))]
        [SerializeField] private AudioSource confirmAudioSource = null;
        [Tooltip("Enable/disable use of the back button with the grid")]
        [SerializeField] private bool backEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(backEnabled))]
        [SerializeField] private AudioSource backAudioSource = null;
        [Tooltip("Enable/disable use of the menu button with the grid")]
        [SerializeField] private bool menuEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(menuEnabled))]
        [SerializeField] private AudioSource menuAudioSource = null;
        [Tooltip("Enable/disable use of the attack button with the grid")]
        [SerializeField] private bool specialEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(specialEnabled))]
        [SerializeField] private AudioSource specialAudioSource = null;
        [Tooltip("Enable/disable use of the pause button with the grid")]
        [SerializeField] private bool pauseEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(pauseEnabled))]
        [SerializeField] private AudioSource pauseAudioSource = null;
        [Tooltip("Enable/disable use of the select button with the grid")]
        [SerializeField] private bool selectEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(selectEnabled))]
        [SerializeField] private AudioSource selectAudioSource = null;

        /// <summary>
        /// True if the grid has been set up, otherwise false
        /// </summary>
        public bool Initialized => this.grid is not null;

        /// <summary>
        /// The area of the grid (width * height)
        /// </summary>
        public int Area => this.gridWidth * this.gridHeight;

        /// <summary>
        /// Width of the grid
        /// </summary>
        public int Width => this.gridWidth;

        /// <summary>
        /// Height of the grid
        /// </summary>
        public int Height => this.gridHeight;

        /// <summary>
        /// The number of options in the grid
        /// </summary>
        public int Count => this.count;

        /// <summary>
        /// Enable/disable use of the confirm button
        /// </summary>
        public bool ConfirmEnabled { get => this.confirmEnabled; set => this.confirmEnabled = value; }

        /// <summary>
        /// Enable/disable use of the back button
        /// </summary>
        public bool BackEnabled { get =>this.backEnabled; set => this.backEnabled = value; }

        /// <summary>
        /// Enable/disable use of the menu button
        /// </summary>
        public bool MenuEnabled { get => this.menuEnabled; set => this.menuEnabled = value; }

        /// <summary>
        /// Enable/disable use of the attack button
        /// </summary>
        public bool SpecialEnabled { get => this.specialEnabled; set => this.specialEnabled = value; }

        /// <summary>
        /// Enable/disable use of the pause button
        /// </summary>
        public bool PauseEnabled { get => this.pauseEnabled; set => this.pauseEnabled = value; }

        /// <summary>
        /// Enable/disable use of the select button
        /// </summary>
        public bool SelectEnabled { get => this.selectEnabled; set => this.selectEnabled = value; }

        public GridOption CurrentOption
        {
            get => At(this.cursorPosition);
            private set => this.grid[this.cursorPosition.x, this.cursorPosition.y] = value;
        }

        public Vector2Int CursorPosition
        {
            get => this.cursorPosition;
            set
            {
                if (!ValidPosition(value))
                {
                    Debug.LogError($"[OptionsGrid:CursorPosition:Get] Tried to set the cursor position but the new position was outside the bounds of the grid -> ({value.x},{value.y})");
                    return;
                }

                if (!Exists(value))
                {
                    Debug.LogWarning($"[OptionsGrid:CursorPosition:Set] Tried to set the cursor position but no grid option exists at that position -> ({value.x},{value.y})");
                    return;
                }

                GridOption previousOption = this.CurrentOption;
                Vector2Int previousPosition = this.CursorPosition;
                int previousPosition1D = this.CursorPosition1D;

                this.cursorPosition = value;
                if (this.Interactable)
                    this.CurrentOption.SetCursor(true);

                NavInfo info = new()
                {
                    cursorPosition = this.CursorPosition,
                    cursorPosition1D = this.CursorPosition1D,
                    cursorPositionPrev = this.CursorPosition,
                    cursorPosition1DPrev = this.CursorPosition1D,
                };

                if (previousOption && !previousOption.Equals(this.CurrentOption))
                {
                    previousOption.SetCursor(false);
                    info.cursorPositionPrev = previousPosition;
                    info.cursorPosition1DPrev = previousPosition1D;
                }

                this.onNavigate.Invoke(info);
                this.CurrentOption.OnNavigate();
            }
        }

        public int CursorPosition1D => this.instantiationAxis switch
        {
            Axis.Horizontal => (this.gridWidth * this.cursorPosition.y) + this.cursorPosition.x,
            Axis.Vertical => (this.gridHeight * this.cursorPosition.x) + this.cursorPosition.y,
            _ => throw new Exception($"[OptionsGrid:CursorPosition1D] The instantiation axis is set to an invalid value"),
        };

        private GridOption[,] grid = null;
        private int count = 0;
        private Vector2Int cursorPosition = new(0, 0);
        private Vector2Int head = new(0, 0);

        #region Public Methods
        public GridOption At(Vector2Int position)
        {
            if (!ValidPosition(position))
                return null;

            return this.grid[position.x, position.y];
        }

        public bool Exists(Vector2Int position) => At(position) != null;

        public bool ValidPosition(Vector2Int position) => position.x < this.gridWidth && position.x >= 0 && position.y < this.gridHeight && position.y >= 0;

        public void SetCursorAtPosition(Vector2Int cursorPosition, bool value)
        {
            if (!this.Initialized)
            {
                Debug.LogWarning($"[OptionsGrid:SetCursorAtPosition] Tried to set the cursor to {value} at position ({cursorPosition.x}, {cursorPosition.y}) but the grid was null.");
                return;
            }

            if (!ValidPosition(cursorPosition))
            {
                Debug.LogError($"[OptionsGrid:SetCursorAtPosition] Tried to set the cursor to {value} at position ({cursorPosition.x},{cursorPosition.y}) but the position was outside the bounds of the grid.");
                return;
            }

            if (!Exists(cursorPosition))
            {
                Debug.LogError($"[OptionsGrid:SetCursorAtPosition] Tried to set the cursor to {value} at position ({cursorPosition.x},{cursorPosition.y}) but no option exists at that position.");
                return;
            }

            At(cursorPosition).SetCursor(value);
        }

        public void OnNavigate() => this.CursorPosition = this.CursorPosition;

        /// <summary>
        /// Sets up a new grid, clearing any previous data
        /// </summary>
        public void Setup()
        {
            //Clean up anything that could be left over
            Clear();

            //Create the element data structure
            this.grid = new GridOption[this.gridWidth, this.gridHeight];
        }

        /// <summary>
        /// Sets up a new grid, clearing any previous data
        /// </summary>
        /// <param name="width">The new grid width</param>
        /// <param name="height">The new grid height</param>
        public void Setup(int width, int height)
        {
            //Clean up anything that could be left over
            Clear();

            //Set the width and height
            this.gridWidth = width;
            this.gridHeight = height;

            //Create the element data structure
            this.grid = new GridOption[this.gridWidth, this.gridHeight];
        }

        /// <summary>
        /// Sets up a new grid if a grid is not already set up and loads a collection of options into it
        /// </summary>
        /// <param name="gridOptions">The grid options to add</param>
        public void LoadOptions(IEnumerable<GridOption> gridOptions)
        {
            if (this.Width > 0 && this.Height > 0)
                Setup(this.Width, this.Height);

            if (gridOptions is not null)
                foreach (GridOption option in gridOptions)
                    Add(option);
        }

        /// <summary>
        /// Sets up a new grid if a grid is not already set up and loads a collection of options into it
        /// </summary>
        /// <param name="datas">The grid options to create</param>
        public void LoadOptions(IEnumerable<GridOption.Data> datas)
        {
            if (this.Width > 0 && this.Height > 0)
                Setup(this.Width, this.Height);

            if (datas is not null)
                foreach (GridOption.Data data in datas)
                    Add(data);
        }

        /// <summary>
        /// Instantiates and adds an option to the grid
        /// </summary>
        /// <param name="optionData">The data for the option</param>
        /// <returns>The UI option object</returns>
        public GridOption Add(GridOption.Data optionData)
        {
            if (!this.optionPrefab)
            {
                Debug.LogError("[OptionsGrid:Add] Tried to add an option to the grid from a GridOption.Data but the option prefabID was null");
                return null;
            }

            if (this.count + 1 > this.Area)
            {
                Debug.LogWarning("[OptionsGrid:Add] Tried to add but the grid was full");
                return null;
            }

            //Create and set up the added element
            GridOption option = Instantiate(this.optionPrefab);
            this.grid[this.head.x, this.head.y] = option;
            option.transform.SetParent(this.container);
            option.transform.localScale = Vector3.one;
            option.Setup(optionData);

            this.head = Increment(this.head);

            //Increase the count
            this.count++;

            return option;
        }

        /// <summary>
        /// Adds an existing option to the grid
        /// </summary>
        /// <param name="option">The existing option</param>
        /// <returns>true if added successfully, otherwise false</returns>
        public bool Add(GridOption option)
        {
            if (this.count + 1 > this.Area)
            {
                Debug.LogWarning("[OptionsGrid:Add] Tried to add but the grid was full");
                return false;
            }

            //Create and set up the added element
            this.grid[this.head.x, this.head.y] = option;

            this.head = Increment(this.head);

            //Increase the count
            this.count++;

            return true;
        }

        /// <summary>
        /// Removes the option selected by the cursor from the grid
        /// </summary>
        public void Remove()
        {
            if (this.count < 1)
            {
                Debug.LogWarning("[OptionsGrid:Remove] Tried to remove but the grid was empty");
                return;
            }

            Destroy(this.CurrentOption.gameObject);
            this.CurrentOption = null;
            this.count--;

            if (this.count < 1)
            {
                this.CursorPosition = new Vector2Int(0, 0);
                this.head = new Vector2Int(0, 0);
            }


            int maxi = this.gridHeight;
            int maxj = this.gridWidth;
            if (this.instantiationAxis == Axis.Vertical)
            {
                maxi = this.gridWidth;
                maxj = this.gridHeight;
            }

            Queue<GridOption> queue = new();
            for (int i = 0; i < maxi; i++)
            {
                for (int j = 0; j < maxj; j++)
                {
                    int x = j;
                    int y = i;
                    if (this.instantiationAxis == Axis.Vertical)
                    {
                        x = i;
                        y = j;
                    }

                    if (this.grid[x, y] != null)
                        queue.Enqueue(this.grid[x, y]);
                }
            }

            this.grid = new GridOption[this.gridWidth, this.gridHeight];

            for (int i = 0; i < maxi; i++)
            {
                for (int j = 0; j < maxj; j++)
                {
                    int x = j;
                    int y = i;
                    if (this.instantiationAxis == Axis.Vertical)
                    {
                        x = i;
                        y = j;
                    }

                    if (queue.Count > 0)
                        this.grid[x, y] = queue.Dequeue();
                }
            }

            this.head = Decrement(this.head);

            if (this.CursorPosition == this.head)
                this.CursorPosition = Decrement(this.CursorPosition);

            this.CurrentOption.SetCursor(true);
        }

        /// <summary>
        /// Clears all options and resets all option dependent data from the grid
        /// </summary>
        public void Clear() {
            if (!this.Initialized)
                return;

            //Remove all elements in the grid
            for (int i = 0; i < this.gridWidth; i++)
                for (int j = 0; j < this.gridHeight; j++)
                    if (this.grid[i, j] != null)
                        Destroy(this.grid[i, j].gameObject);

            //Reset all private members that are dependent on grid elements
            this.count = 0;
            this.head = new Vector2Int(0, 0);
        }

        /// <summary>
        /// Resets the cursor at the start of the grid
        /// </summary>
        public void SetCursorToFirst() => SetCursorTo(new Vector2Int(0, 0), "First", Increment);

        /// <summary>
        /// Resets the cursor at the nearest valid position to the curent cursor position
        /// </summary>
        public void SetCursorToNearest() => SetCursorTo(this.CursorPosition, "Nearest",
            this.CursorPosition1D > this.Count / 2 ? Decrement : Increment);

        /// <summary>
        /// Resets the cursor at the head of the grid
        /// </summary>
        public void SetCursorToLast() => SetCursorTo(Decrement(this.head), "Last", Decrement);

        /// <summary>
        /// Invokes the input button event of the current option
        /// </summary>
        /// <param name="inputButton">The input button event to trigger</param>
        public void InvokeEvent(InputButton inputButton)
        {
            if (!this.Initialized)
            {
                Debug.LogError($"[OptionsGrid:InvokeEvent] Tried to invoke the {inputButton} event of {this.name} at position ({this.CursorPosition.x}, {this.CursorPosition.y}) but the grid was null");
                return;
            }

            if (this.Interactable && this.gameObject.activeSelf && ButtonEnabled(inputButton) && this.Count > 0)
            {
                this.CurrentOption.InvokeEvent(inputButton);
                Utilities.Audio.PlayAudioSource(GetAudioSource(inputButton));
            }
        }

        /// <summary>
        /// Navigate through the grid
        /// </summary>
        /// <param name="direction">The direction of navigation</param>
        public void Navigate(InputDirection direction)
        {
            if (!this.Interactable || !this.gameObject.activeSelf || this.Count < 1)
                return;

            Vector2Int newCursorPosition = this.CursorPosition;
            Vector2Int bfsStartingPosition = this.CursorPosition;

            GridOption originalOption = this.CurrentOption;
            switch (direction)
            {
                case InputDirection.Up:
                    do newCursorPosition.y = Decrement(this.CursorPosition.y, this.gridHeight);
                    while (this.CurrentOption == null || !this.CurrentOption.Interactable);

                    bfsStartingPosition.y = Decrement(this.CursorPosition.y, this.gridHeight);
                    if (this.CurrentOption == originalOption)
                        newCursorPosition = DirectionalBFS(bfsStartingPosition, direction);
                    break;
                case InputDirection.Left:
                    do newCursorPosition.x = Decrement(this.CursorPosition.x, this.gridWidth);
                    while (this.CurrentOption == null || !this.CurrentOption.Interactable);

                    bfsStartingPosition.x = Decrement(this.CursorPosition.x, this.gridWidth);
                    if (this.CurrentOption == originalOption)
                        newCursorPosition = DirectionalBFS(bfsStartingPosition, direction);
                    break;
                case InputDirection.Down:
                    do newCursorPosition.y = Increment(this.CursorPosition.y, this.gridHeight);
                    while (this.CurrentOption == null || !this.CurrentOption.Interactable);

                    bfsStartingPosition.y = Increment(this.CursorPosition.y, this.gridHeight);
                    if (this.CurrentOption == originalOption)
                        newCursorPosition = DirectionalBFS(bfsStartingPosition, direction);
                    break;
                case InputDirection.Right:
                    do newCursorPosition.x = Increment(this.CursorPosition.x, this.gridWidth);
                    while (this.CurrentOption == null || !this.CurrentOption.Interactable);

                    bfsStartingPosition.x = Increment(this.CursorPosition.x, this.gridWidth);
                    if (this.CurrentOption == originalOption)
                        newCursorPosition = DirectionalBFS(bfsStartingPosition, direction);
                    break;
                default:
                    Debug.LogError("[OptionsGrid:Navigate] Invalid direction");
                    break;
            }

            this.CursorPosition = newCursorPosition;

            Utilities.Audio.PlayAudioSource(this.navigateAudioSource);
            
        }
        #endregion

        #region Public Overrides
        public override void UtilityInitialize()
        {
            base.UtilityInitialize();

            if (Exists(this.CursorPosition))
                this.CurrentOption.SetCursor(true);
            else
                SetCursorToNearest();
        }
        #endregion

        #region Private Methods
        private int Increment(int value, int size)
        {
            int field = value;

            if (field == size - 1)
            {
                field = 0;
            }
            else
            {
                field += 1;
            }

            return field;
        }

        private Vector2Int Increment(Vector2Int vector)
        {
            Vector2Int v = vector;
            switch (this.instantiationAxis)
            {
                case Axis.Horizontal:
                    if (v.x + 1 >= this.gridWidth)
                    {
                        v.x = 0;
                        v.y++;
                    }
                    else
                    {
                        v.x++;
                    }
                    break;
                case Axis.Vertical:
                    if (v.y + 1 >= this.gridHeight)
                    {
                        v.y = 0;
                        v.x++;
                    }
                    else
                    {
                        v.y++;
                    }
                    break;
            }

            return v;
        }

        private int Decrement(int value, int size)
        {
            int field = value;

            if (field == 0)
            {
                field = size - 1;
            }
            else
            {
                field -= 1;
            }

            return field;

        }

        private Vector2Int Decrement(Vector2Int vector)
        {
            Vector2Int v = vector;
            switch (this.instantiationAxis)
            {
                case Axis.Horizontal:
                    if (v.x - 1 < 0)
                    {
                        v.x = this.gridWidth - 1;
                        v.y--;
                    }
                    else
                    {
                        v.x--;
                    }
                    break;
                case Axis.Vertical:
                    if (v.y - 1 < 0)
                    {
                        v.y = this.gridHeight - 1;
                        v.x--;
                    }
                    else
                    {
                        v.y--;
                    }
                    break;
            }

            return v;
        }

        private void SetCursorTo(Vector2Int start, string debug, Func<Vector2Int, Vector2Int> modifier)
        {
            if (!this.Initialized)
            {
                Debug.LogError($"[OptionsGrid:SetCursorTo{debug}] Tried to set the cursor to {debug.ToLower()} but the grid was null");
                return;
            }

            if (this.Count < 1)
                return;

            Vector2Int newPosition = start;
            GridOption option = At(newPosition);
            while (option ? !option.Interactable : false)
            {
                newPosition = modifier(newPosition);
                option = At(newPosition);
            }

            this.CursorPosition = newPosition;
        }

        private AudioSource GetAudioSource(InputButton inputButton) => inputButton switch
        {
            InputButton.Confirm => this.confirmAudioSource,
            InputButton.Back => this.backAudioSource,
            InputButton.Menu => this.menuAudioSource,
            InputButton.Special => this.specialAudioSource,
            InputButton.Pause => this.pauseAudioSource,
            InputButton.Select => this.selectAudioSource,
            _ => throw new ArgumentException("[OptionsGrid:GetAudioSource] InputButton was null or invalid")
        };

        private bool ButtonEnabled(InputButton inputButton) => inputButton switch
        {
            InputButton.Confirm => this.confirmEnabled,
            InputButton.Back => this.backEnabled,
            InputButton.Menu => this.menuEnabled,
            InputButton.Special => this.specialEnabled,
            InputButton.Pause => this.pauseEnabled,
            InputButton.Select => this.selectEnabled,
            _ => throw new ArgumentException("[OptionsGrid:ButtonEnabled] InputButton was null or invalid")
        };

        private InputDirection InvertDirection(InputDirection direction) => direction switch
        {
            InputDirection.Up => InputDirection.Down,
            InputDirection.Left => InputDirection.Right,
            InputDirection.Down => InputDirection.Up,
            InputDirection.Right => InputDirection.Left,
            _ => throw new ArgumentException("[OptionsGrid:ButtonEnabled] InputDirection was null or invalid")
        };

        private Vector2Int DirectionalBFS(Vector2Int startingPosition, InputDirection direction)
        {
            bool[,] visited = new bool[this.gridWidth, this.gridHeight];

            if (direction == InputDirection.Up || direction == InputDirection.Down)
            {
                for (int i = 0; i < this.gridWidth; i++)
                    visited[i, this.CursorPosition.y] = true;
            }

            if (direction == InputDirection.Left || direction == InputDirection.Right)
            {
                for (int i = 0; i < this.gridHeight; i++)
                    visited[this.CursorPosition.x, i] = true;
            }

            visited[startingPosition.x, startingPosition.y] = true;

            Queue<Vector2Int> bfsq = new();
            bfsq.Enqueue(startingPosition);

            Vector2Int current = startingPosition;
            while (bfsq.Count > 0)
            {
                current = bfsq.Dequeue();
                visited[current.x, current.y] = true;
                if (Exists(current) && At(current).Interactable)
                    break;

                foreach (InputDirection branchDirection in Enum.GetValues(typeof(InputDirection)))
                {
                    if (branchDirection == InvertDirection(direction))
                        continue;

                    Vector2Int toQueue = current;
                    switch (branchDirection)
                    {
                        case InputDirection.Up:
                            toQueue.y = Decrement(toQueue.y, this.gridHeight);
                            break;
                        case InputDirection.Left:
                            toQueue.x = Decrement(toQueue.x, this.gridWidth);
                            break;
                        case InputDirection.Down:
                            toQueue.y = Increment(toQueue.y, this.gridHeight);
                            break;
                        case InputDirection.Right:
                            toQueue.x = Increment(toQueue.x, this.gridWidth);
                            break;
                    }

                    if (visited[toQueue.x, toQueue.y])
                        continue;

                    bfsq.Enqueue(toQueue);
                }
            }

            if (!Exists(current) || !At(current).Interactable)
                current = this.CursorPosition;

            return current;
        }
        #endregion
    }
}
