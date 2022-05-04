﻿using System.Collections.Generic;
using UnityEngine;
using System;
using BF2D.Enums;
using BF2D.Attributes;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BF2D.UI
{
    public class UIOptionsGrid : UIUtility {

        [Header("Grid")]
        [SerializeField] private UIOption optionPrefab = null;
        [Tooltip("The container for grid options")]
        [SerializeField] private LayoutGroup container = null;
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
        public class OnNavigate : UnityEvent<string> { }
        [SerializeField]
        private OnNavigate onNavigateEvent = new OnNavigate();


        [SerializeField] private AudioSource navigateAudioSource = null;
        [Tooltip("Enable/disable use of the confirm button with the grid")]
        [SerializeField] private bool confirmEnabled = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(confirmEnabled))]
        [SerializeField] private AudioSource confirmAudioSource = null;
        [Tooltip("Enable/disable use of the back button with the grid")]
        [SerializeField] private bool backEnabled = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(backEnabled))]
        [SerializeField] private AudioSource backAudioSource = null;
        [Tooltip("Enable/disable use of the menu button with the grid")]
        [SerializeField] private bool menuEnabled = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(menuEnabled))]
        [SerializeField] private AudioSource menuAudioSource = null;
        [Tooltip("Enable/disable use of the attack button with the grid")]
        [SerializeField] private bool attackEnabled = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(attackEnabled))]
        [SerializeField] private AudioSource attackAudioSource = null;
        [Tooltip("Enable/disable use of the pause button with the grid")]
        [SerializeField] private bool pauseEnabled = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(pauseEnabled))]
        [SerializeField] private AudioSource pauseAudioSource = null;
        [Tooltip("Enable/disable use of the select button with the grid")]
        [SerializeField] private bool selectEnabled = true;
        [ShowIf(ActionOnConditionFail.DontDraw, ConditionOperator.And, nameof(selectEnabled))]
        [SerializeField] private AudioSource selectAudioSource = null;

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

        private UIOption[,] grid;
        private int count = 0;
        public Vector2Int CursorPosition { get { return this.cursorPosition; } }
        private Vector2Int cursorPosition = new Vector2Int(0, 0);
        private Vector2Int head = new Vector2Int(0, 0);

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
        public UIOption Add(UIOptionData optionData)
        {
            UIOption option = null;

            //Base case
            if (this.count + 1 > Size)
            {
                Debug.LogWarning("[UIOptionsGrid] Tried to add but the grid was full");
                return null;
            }

            //Create and set up the added element
            option = Instantiate(this.optionPrefab);
            this.grid[this.head.x, this.head.y] = option;
            option.transform.SetParent(this.container.transform);
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

            return option;
        }

        /// <summary>
        /// Removes the option selected by the cursor from the grid
        /// </summary>
        /// <returns>True if the option was removed successfully, otherwise returns false</returns>
        public bool Remove()
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
                this.cursorPosition = new Vector2Int(0, 0);
                this.head = new Vector2Int(0, 0);
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

        /// <summary>
        /// Clears all options and resets all option dependent data from the grid
        /// </summary>
        public void Clear() {
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
            foreach (UIOption option in this.grid)
            {
                option.SetCursor(false);
            }

            this.grid[0, 0].SetCursor(true);
        }

        public void InvokeEvent(InputButton inputButton)
        {
            if (this.Interactable && this.gameObject.activeSelf && ButtonEnabled(inputButton) && this.count > 0)
            {
                this.grid[this.cursorPosition.x, this.cursorPosition.y].InvokeEvent(inputButton);
                PlayAudioSource(GetAudioSource(inputButton));
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
                        Debug.LogError("[UIOptionsGrid] Invalid direction");
                        break;
                }

                SetCursorAtPosition(this.cursorPosition, true);

                PlayAudioSource(this.navigateAudioSource);

                this.onNavigateEvent.Invoke(this.grid[this.cursorPosition.x, this.cursorPosition.y].gameObject.name);
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
                _ => throw new ArgumentException("[UIOptionsGrid] InputButton was null or invalid")
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
                _ => throw new ArgumentException("[UIOptionsGrid] InputButton was null or invalid")
            };
        }
        #endregion
    }
}