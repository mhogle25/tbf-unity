using System.Collections.Generic;
using UnityEngine;
using System;
using BF2D.Enums;
using BF2D.Attributes;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BF2D.UI
{
    public class OptionsGrid : UIUtility
    {
        [SerializeField] private bool staticGrid = true;
        [Header("Grid")]
        [ShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.NAND, nameof(staticGrid))]
        [SerializeField] private GridOption optionPrefab = null;
        [Tooltip("The container for grid options")]
        [SerializeField] private Transform container = null;
        [Tooltip("Determines the direction that elements will be populated in")]
        [SerializeField] private Axis instantiationAxis = Axis.Horizontal;
        [SerializeField] private int gridWidth = 1;
        [SerializeField] private int gridHeight = 1; 
        
        public OnNavigate OnNavigateEvent
        {
            get
            {
                return this.onNavigateEvent;
            }
        }

        [Serializable]
        public class OnNavigate : UnityEvent<int> { }
        [SerializeField]
        private OnNavigate onNavigateEvent = new();


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
        [SerializeField] private bool attackEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(attackEnabled))]
        [SerializeField] private AudioSource attackAudioSource = null;
        [Tooltip("Enable/disable use of the pause button with the grid")]
        [SerializeField] private bool pauseEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(pauseEnabled))]
        [SerializeField] private AudioSource pauseAudioSource = null;
        [Tooltip("Enable/disable use of the select button with the grid")]
        [SerializeField] private bool selectEnabled = false;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.AND, nameof(selectEnabled))]
        [SerializeField] private AudioSource selectAudioSource = null;

        /// <summary>
        /// Whether this options grid is meant to handle statically defined options. If false, this grid is used for dynamically populating options.
        /// </summary>
        public bool StaticContent { get { return this.staticGrid; } }

        /// <summary>
        /// The area of the grid (width * height)
        /// </summary>
        public int Size { get { return this.gridWidth * this.gridHeight; } }

        /// <summary>
        /// Width of the grid
        /// </summary>
        public int Width { get { return this.gridWidth; } }

        /// <summary>
        /// Height of the grid
        /// </summary>
        public int Height { get { return this.gridHeight; } }

        /// <summary>
        /// The number of options in the grid
        /// </summary>
        public int Count { get { return this.count; } }

        /// <summary>
        /// Enable/disable use of the confirm button
        /// </summary>
        public bool ConfirmEnabled { get { return this.confirmEnabled; } set { this.confirmEnabled = value; } }

        /// <summary>
        /// Enable/disable use of the menu button
        /// </summary>
        public bool BackEnabled { get { return this.backEnabled; } set { this.backEnabled = value; } }

        /// <summary>
        /// Enable/disable use of the menu button
        /// </summary>
        public bool MenuEnabled { get { return this.menuEnabled; } set { this.menuEnabled = value; } }
        
        /// <summary>
        /// Enable/disable use of the menu button
        /// </summary>
        public bool AttackEnabled { get { return this.attackEnabled; } set { this.attackEnabled = value; } }

        /// <summary>
        /// Enable/disable use of the menu button
        /// </summary>
        public bool PauseEnabled { get { return this.pauseEnabled; } set { this.pauseEnabled = value; } }

        /// <summary>
        /// Enable/disable use of the menu button
        /// </summary>
        public bool SelectEnabled { get { return this.selectEnabled; } set { this.selectEnabled = value; } }

        private GridOption[,] grid = null;
        private int count = 0;
        public Vector2Int CursorPosition { get { return this.cursorPosition; } }
        public int CursorPosition1D
        {
            get
            {
                switch (this.instantiationAxis)
                {
                    case Axis.Horizontal:
                        return (this.gridWidth * this.cursorPosition.y) + this.cursorPosition.x;
                    case Axis.Vertical:
                        return (this.gridHeight * this.cursorPosition.x) + this.cursorPosition.y;
                    default:
                        throw new Exception($"[OptionsGrid] The instantiation axis is set to an invalid value: {this.instantiationAxis}");
                }
            }
        }
        private Vector2Int cursorPosition = new(0, 0);
        private Vector2Int head = new(0, 0);

        private void Awake()
        {
            if (!this.staticGrid)
                return;

            UIOption[] options = this.container.GetComponentsInChildren<UIOption>();

            if (this.gridWidth > 0 && this.gridHeight > 0)
            {
                //Create the element data structure
                this.grid = new UIOption[this.gridWidth, this.gridHeight];
            }

            if (options != null && options.Length > 0)
            {
                foreach (UIOption option in options)
                {
                    Add(option);
                }
            }
        }

        #region Public Methods

        /// <summary>
        /// Sets up a new grid, clearing any previous data
        /// </summary>
        /// <param name="width">The new grid width</param>
        /// <param name="height">The new grid height</param>
        public void Setup(int width, int height)
        {
            if (this.staticGrid)
            {
                Debug.LogWarning($"[OptionsGrid] Tried to dynamically set up the options grid '{this.gameObject.name}' when it was set to static");
                return;
            }

            //Clean up anything that could be left over
            Clear();

            //Set the width and height
            this.gridWidth = width;
            this.gridHeight = height;

            //Create the element data structure
            this.grid = new UIOption[this.gridWidth, this.gridHeight];
        }

        /// <summary>
        /// Instantiates and adds an option to the grid
        /// </summary>
        /// <param name="optionData">The data for the option</param>
        /// <returns>The UI option object</returns>
        public GridOption Add(GridOption.Data optionData)
        {

            if (this.staticGrid)
            {
                Debug.LogWarning("[OptionsGrid] Tried to add an option to the options grid while it was set to static");
                return null;
            }

            GridOption option = null;

            //Base case
            if (this.count + 1 > Size)
            {
                Debug.LogWarning("[OptionsGrid] Tried to add but the grid was full");
                return null;
            }

            //Create and set up the added element
            option = Instantiate(this.optionPrefab);
            this.grid[this.head.x, this.head.y] = option;
            option.transform.SetParent(this.container.transform);
            option.transform.localScale = Vector3.one;
            option.Setup(optionData);

            //If the cursor wasn't already enabled, enable it
            if (this.count < 1)
            {
                SetCursorAtPosition(this.head, true);
            }

            this.head = Increment(this.head);

            //Increase the count
            this.count++;

            return option;
        }

        /// <summary>
        /// Removes the option selected by the cursor from the grid
        /// </summary>
        /// <returns>True if the option was removed successfully, otherwise returns false</returns>
        public void Remove()
        {
            //Base Case
            if (this.count < 1)
            {
                Debug.LogWarning("[OptionsGrid] Tried to remove but the grid was empty");
            }

            Destroy(this.grid[this.cursorPosition.x, this.cursorPosition.y].gameObject);
            this.grid[this.cursorPosition.x, this.cursorPosition.y] = null;
            this.count--;

            if (this.count < 1)
            {
                this.cursorPosition = new Vector2Int(0, 0);
                this.head = new Vector2Int(0, 0);
            }


            int maxi = this.gridHeight;
            int maxj = this.gridWidth;
            if (this.instantiationAxis == Axis.Vertical)
            {
                maxi = this.gridWidth;
                maxj = this.gridHeight;
            }

            Queue<GridOption> queue = new Queue<GridOption>();
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
                    {
                        queue.Enqueue(this.grid[x, y]);
                    }
                }
            }

            this.grid = new UIOption[this.gridWidth, this.gridHeight];

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
                    {
                        this.grid[x, y] = queue.Dequeue();
                    }
                }
            }

            this.head = Decrement(this.head);

            if (this.cursorPosition == this.head)
            {
                this.cursorPosition = Decrement(this.cursorPosition);
            }

            SetCursorAtPosition(this.cursorPosition, true);
        }

        /// <summary>
        /// Clears all options and resets all option dependent data from the grid
        /// </summary>
        public void Clear() {
            if (this.grid is null)
                return;

            //Remove all elements in the grid
            for (int i = 0; i < this.gridWidth; i++)
                for (int j = 0; j < this.gridHeight; j++)
                    if (this.grid[i, j] != null)
                    {
                        Destroy(this.grid[i, j].gameObject);
                    }

            //Reset all private members that are dependent on grid elements
            this.count = 0;
            this.cursorPosition = new Vector2Int(0, 0);
            this.head = new Vector2Int(0, 0);
        }

        /// <summary>
        /// Reset the cursor to be at the head of the grid
        /// </summary>
        public void SetCursorAtHead()
        {
            if (this.grid is null)
            {
                Debug.LogError("[OptionsGrid] Tried to set the cursor at head but the grid was null");
            }

            foreach (UIOption option in this.grid)
            {
                if (option is null)
                    continue;
                option.SetCursor(false);
            }

            this.grid[0, 0].SetCursor(true);
        }

        public void InvokeEvent(InputButton inputButton)
        {
            if (this.grid is null)
            {
                Debug.LogError($"[OptionsGrid] Tried to invoke the {inputButton} event at position ({this.cursorPosition.x}, {this.cursorPosition.y}) but the grid was null");
                return;
            }

            if (this.Interactable && this.gameObject.activeSelf && ButtonEnabled(inputButton) && this.count > 0)
            {
                this.grid[this.cursorPosition.x, this.cursorPosition.y].InvokeEvent(inputButton);
                BF2D.Utilities.Audio.PlayAudioSource(GetAudioSource(inputButton));
            }
        }

        /// <summary>
        /// Navigate through the grid
        /// </summary>
        /// <param name="direction">The direction of navigation</param>
        public void Navigate(InputDirection direction)
        {
            if (this.Interactable && this.gameObject.activeSelf && this.count > 0)
            {
                if (this.grid[this.cursorPosition.x, this.cursorPosition.y] != null)
                    SetCursorAtPosition(this.cursorPosition, false);

                switch (direction)
                {
                    case InputDirection.Up:
                        while (this.grid[this.cursorPosition.x, this.cursorPosition.y = Decrement(this.cursorPosition.y, this.gridHeight)] == null);
                        break;
                    case InputDirection.Left:
                        while (this.grid[this.cursorPosition.x = Decrement(this.cursorPosition.x, this.gridWidth), this.cursorPosition.y] == null) ;
                        break;
                    case InputDirection.Down:
                        while (this.grid[this.cursorPosition.x, this.cursorPosition.y = Increment(this.cursorPosition.y, this.gridHeight)] == null) ;
                        break;
                    case InputDirection.Right:
                        while (this.grid[this.cursorPosition.x = Increment(this.cursorPosition.x, this.gridWidth), this.cursorPosition.y] == null);
                        break;
                    default:
                        Debug.LogError("[OptionsGrid] Invalid direction");
                        break;
                }

                SetCursorAtPosition(this.cursorPosition, true);

                BF2D.Utilities.Audio.PlayAudioSource(this.navigateAudioSource);

                this.onNavigateEvent.Invoke(CursorPosition1D);
            }
        }
        #endregion

        #region Private Methods
        private bool Add(UIOption option)
        {
            //Base case
            if (this.count + 1 > Size)
            {
                Debug.LogWarning("[OptionsGrid] Tried to add but the grid was full");
                return false;
            }

            //Create and set up the added element
            this.grid[this.head.x, this.head.y] = option;

            //If the cursor did not already exist, enable it
            if (this.count < 1)
            {
                SetCursorAtPosition(this.head, true);
            }

            this.head = Increment(this.head);

            //Increase the count
            this.count++;

            return true;
        }

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

        private void SetCursorAtPosition(Vector2Int cursorPosition, bool value)
        {
            if (this.grid is null)
            {
                Debug.LogWarning($"[OptionsGrid] Tried to set the cursor to {value} at position ({cursorPosition.x}, {cursorPosition.y}) but the grid was null");
                return;
            }

            this.grid[cursorPosition.x, cursorPosition.y].SetCursor(value);
        }

        private AudioSource GetAudioSource(InputButton inputButton)
        {
            return inputButton switch
            {
                InputButton.Confirm => this.confirmAudioSource,
                InputButton.Back => this.backAudioSource,
                InputButton.Menu => this.menuAudioSource,
                InputButton.Attack => this.attackAudioSource,
                InputButton.Pause => this.pauseAudioSource,
                InputButton.Select => this.selectAudioSource,
                _ => throw new ArgumentException("[OptionsGrid] InputButton was null or invalid")
            };
        }

        private bool ButtonEnabled(InputButton inputButton)
        {
            return inputButton switch
            {
                InputButton.Confirm => this.confirmEnabled,
                InputButton.Back => this.backEnabled,
                InputButton.Menu => this.menuEnabled,
                InputButton.Attack => this.attackEnabled,
                InputButton.Pause => this.pauseEnabled,
                InputButton.Select => this.selectEnabled,
                _ => throw new ArgumentException("[OptionsGrid] InputButton was null or invalid")
            };
        }
        #endregion
    }
}
