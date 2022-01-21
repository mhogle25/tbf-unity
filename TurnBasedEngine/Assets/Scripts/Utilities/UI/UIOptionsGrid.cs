using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BF2D.UI
{
    public class UIOptionsGrid : UIUtility {

        [Header("Grid")]
        [SerializeField] private UIOption optionPrefab = null;
        [Tooltip("The container for grid options")]
        [SerializeField] private RectTransform container = null;
        [Tooltip("Determines the direction that elements will be populated in")]
        [SerializeField] private Axis instantiationAxis = Axis.Horizontal;
        [SerializeField] private int gridWidth = 1;
        [SerializeField] private int gridHeight = 1;
        [Tooltip("Enable/disable use of the confirm button")]
        [SerializeField] private bool confirmEnabled = true;

        [Header("Audio")]
        [SerializeField] private AudioSource navigateAudioSource = null;
        [SerializeField] private AudioSource confirmAudioSource = null;

        /// <summary>
        /// The area of the grid (width * height)
        /// </summary>
        public int Size { get { return this.gridWidth * this.gridHeight; } }

        /// <summary>
        /// The number of options in the grid
        /// </summary>
        public int Count { get { return this.count; } }

        /// <summary>
        /// Enable/disable use of the confirm button
        /// </summary>
        public bool ConfirmEnabled { get { return this.confirmEnabled; } set { this.confirmEnabled = value; } }

        private UIOption[,] grid;
        private int count = 0;
        private IntVector2 cursorPosition = new IntVector2(0, 0);
        private IntVector2 head = new IntVector2(0, 0);

        private void Awake()
        {
            if (this.gridWidth > 0 && this.gridHeight > 0)
            {
                //Create the element data structure
                this.grid = new UIOption[this.gridWidth, this.gridHeight];
            }

            UIOption[] options = this.container.GetComponentsInChildren<UIOption>();

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
            if (this.interactable)
            {
                //Clean up anything that could be left over
                Clear();

                //Set the width and height
                this.gridWidth = width;
                this.gridHeight = height;

                //Create the element data structure
                this.grid = new UIOption[this.gridWidth, this.gridHeight];

            }
        }

        /// <summary>
        /// Instantiates and adds an option to the grid
        /// </summary>
        /// <param name="optionData">The data for the option</param>
        /// <returns>The UI option object</returns>
        public UIOption Add(UIOptionData optionData)
        {
            UIOption option = null;

            if (this.interactable)
            {
                //Base case
                if (this.count + 1 > Size)
                {
                    Debug.LogWarning("[UIOptionsGrid] Tried to add but the grid was full");
                    return null;
                }

                //Create and set up the added element
                option = Instantiate(this.optionPrefab);
                this.grid[this.head.x, this.head.y] = option;
                option.transform.SetParent(this.container);
                option.transform.localScale = Vector3.one;
                option.Setup(optionData);

                //If the cursor did not already exist, enable it
                if (this.count < 1)
                {
                    SetCursorAtPosition(this.head, true);
                }

                this.head = Increment(this.head);

                //Increase the count
                this.count++;
            }

            return option;
        }

        /// <summary>
        /// Removes the option selected by the cursor from the grid
        /// </summary>
        /// <returns>True if the option was removed successfully, otherwise returns false</returns>
        public bool Remove()
        {
            if (this.interactable && this.gameObject.activeSelf)
            {
                //Base Case
                if (this.count < 1)
                {
                    Debug.LogWarning("[UIOptionsGrid] Tried to remove but the grid was empty");
                    return false;
                }

                Destroy(this.grid[this.cursorPosition.x, this.cursorPosition.y].gameObject);
                this.grid[this.cursorPosition.x, this.cursorPosition.y] = null;
                this.count--;

                if (this.count < 1)
                {
                    this.cursorPosition = new IntVector2(0, 0);
                    this.head = new IntVector2(0, 0);
                    return true;
                }


                int maxi = this.gridHeight;
                int maxj = this.gridWidth;
                if (this.instantiationAxis == Axis.Vertical)
                {
                    maxi = this.gridWidth;
                    maxj = this.gridHeight;
                }

                Queue<UIOption> queue = new Queue<UIOption>();
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

                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Clears all options and resets all option dependent data from the grid
        /// </summary>
        public void Clear() {
            if (this.interactable)
            {
                //Remove all elements in the grid
                for (int i = 0; i < this.gridWidth; i++)
                    for (int j = 0; j < this.gridHeight; j++)
                        if (this.grid[i, j] != null)
                        {
                            Destroy(this.grid[i, j].gameObject);
                        }

                //Reset all private members that are dependent on grid elements
                this.count = 0;
                this.cursorPosition = new IntVector2(0, 0);
                this.head = new IntVector2(0, 0);
            }
        }

        /// <summary>
        /// Reset the cursor to be at the head of the grid
        /// </summary>
        public void SetCursorAtHead()
        {
            if (this.interactable)
            {
                foreach (UIOption option in this.grid)
                {
                    option.SetCursor(false);
                }

                this.grid[0, 0].SetCursor(true);
            }
        }

        /// <summary>
        /// Call the confirm event of the selected option
        /// </summary>
        public void Confirm()
        {
            if (this.interactable && this.gameObject.activeSelf && this.confirmEnabled && this.count > 0)
            {
                this.grid[this.cursorPosition.x, this.cursorPosition.y].Confirm();
                PlayAudioSource(this.confirmAudioSource);
            }
        }

        /// <summary>
        /// Navigate through the grid
        /// </summary>
        /// <param name="direction">The direction of navigation</param>
        public void Navigate(MoveDirection direction)
        {
            if (this.interactable && this.gameObject.activeSelf && this.count > 0)
            {
                if (this.grid[this.cursorPosition.x, this.cursorPosition.y] != null)
                    SetCursorAtPosition(this.cursorPosition, false);

                switch (direction)
                {
                    case MoveDirection.Left:
                        while (this.grid[this.cursorPosition.x = NavigationDecrement(this.cursorPosition.x, this.gridWidth), this.cursorPosition.y] == null) ;
                        break;
                    case MoveDirection.Up:
                        while (this.grid[this.cursorPosition.x, this.cursorPosition.y = NavigationDecrement(this.cursorPosition.y, this.gridHeight)] == null) ;
                        break;
                    case MoveDirection.Right:
                        while (this.grid[this.cursorPosition.x = NavigationIncrement(this.cursorPosition.x, this.gridWidth), this.cursorPosition.y] == null) ;
                        break;
                    case MoveDirection.Down:
                        while (this.grid[this.cursorPosition.x, this.cursorPosition.y = NavigationIncrement(this.cursorPosition.y, this.gridHeight)] == null) ;
                        break;
                    default:
                        Debug.LogError("[UIOptionsGrid] Invalid direction");
                        break;
                }

                SetCursorAtPosition(this.cursorPosition, true);

                PlayAudioSource(this.navigateAudioSource);
            }
        }
        #endregion

        #region Private Methods
        private bool Add(UIOption option)
        {
            //Base case
            if (this.count + 1 > Size)
            {
                Debug.LogWarning("[UIOptionsGrid] Tried to add but the grid was full");
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

        /*
        private void Navigation(bool decrementInput, bool incrementInput, Axis axis, int size) {
            
            if (this.count < 1)
            {
                //Debug.Log("[UIOptionsGrid] Tried to navigate but the grid was empty");
                return;
            }

            int tempField;

            if (this.grid[this.cursorPosition.x, this.cursorPosition.y] != null)    
                SetCursorAtPosition(this.cursorPosition, false);

            switch (axis)
            {
                case Axis.Horizontal:
                    tempField = this.cursorPosition.x;
                    while (this.grid[tempField = NavigationHelper(decrementInput, incrementInput, tempField, size), this.cursorPosition.y] == null) ;
                    this.cursorPosition.x = tempField;
                    break;
                case Axis.Vertical:
                    tempField = this.cursorPosition.y;
                    while (this.grid[this.cursorPosition.x, tempField = NavigationHelper(decrementInput, incrementInput, tempField, size)] == null) ;
                    this.cursorPosition.y = tempField;
                    break;
                default:
                    Debug.LogError("[UIOptionsGrid] Invalid Axis");
                    return;
            }

            SetCursorAtPosition(this.cursorPosition, true);

            PlayAudioSource(this.navigateAudioSource);
        }
        */

        private int NavigationDecrement(int value, int size)
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

        private int NavigationIncrement(int value, int size)
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

        private IntVector2 Increment(IntVector2 vector)
        {
            IntVector2 v = vector;
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

        private IntVector2 Decrement(IntVector2 vector)
        {
            IntVector2 v = vector;
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

        private void SetCursorAtPosition(IntVector2 cursorPosition, bool value)
        {
            this.grid[cursorPosition.x, cursorPosition.y].SetCursor(value);
        }
        #endregion
    }
}
