using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.UI
{
    public class UIOptionsGrid : UIUtility {

        [Header("Grid")]
        [SerializeField] private UIOption _optionPrefab = null;
        [Tooltip("The container for grid options")]
        [SerializeField] private RectTransform _container = null;
        [Tooltip("Determines the direction that elements will be populated in")]
        [SerializeField] private Axis _instantiationAxis = Axis.Horizontal;
        [SerializeField] private int _gridWidth = 0;
        [SerializeField] private int _gridHeight = 0;
        [Tooltip("Enable/disable use of the confirm button")]
        [SerializeField] private bool _confirmEnabled = true;

        [Header("Audio")]
        [SerializeField] private AudioSource _navigateAudioSource = null;
        [SerializeField] private AudioSource _errorAudioSource = null;
        [SerializeField] private AudioSource _confirmAudioSource = null;

        /// <summary>
        /// The area of the grid (width * height)
        /// </summary>
        public int Size { get { return _gridWidth * _gridHeight; } }
        /// <summary>
        /// The number of options in the grid
        /// </summary>
        public int Count { get { return _count; } }
        /// <summary>
        /// Enable/disable use of the confirm button
        /// </summary>
        public bool ConfirmEnabled { get { return _confirmEnabled; } set { _confirmEnabled = value; } }

        private UIOption[,] _grid;
        private int _count = 0;
        private IntVector2 _cursorPosition = new IntVector2(0, 0);
        private IntVector2 _head = new IntVector2(0, 0);

        private void Awake()
        {
            if (_gridWidth > 0 && _gridHeight > 0)
            {
                //Create the element data structure
                _grid = new UIOption[_gridWidth, _gridHeight];
            }

            UIOption[] options = _container.GetComponentsInChildren<UIOption>();

            if (options != null && options.Length > 0)
            {
                foreach (UIOption option in options)
                {
                    Add(option);
                }
            }
        }

        private void Update() {
            //Horizontal Listener
            NavigationListener(InputManager.LeftPress, InputManager.RightPress, Axis.Horizontal, _gridWidth);
            //Vertical Listener
            NavigationListener(InputManager.UpPress, InputManager.DownPress, Axis.Vertical, _gridHeight);

            if (_confirmEnabled)
                ConfirmListener();
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
            _gridWidth = width;
            _gridHeight = height;

            //Create the element data structure
            _grid = new UIOption[_gridWidth, _gridHeight];
        }

        /// <summary>
        /// Instantiates and adds an option to the grid
        /// </summary>
        /// <param name="optionData">The data for the option</param>
        /// <returns>True if the option was added successfully, otherwise returns false</returns>
        public bool Add(UIOptionData optionData)
        {
            //Base case
            if (_count + 1 > Size)
            {
                Debug.Log("[UIOptionsGrid] Tried to add but the grid was full");
                return false;
            }

            //Create and set up the added element
            UIOption option = Instantiate(_optionPrefab);
            _grid[_head.x, _head.y] = option;
            option.transform.SetParent(transform);
            option.transform.localScale = Vector3.one;
            option.Setup(optionData);

            //If the cursor did not already exist, enable it
            if (_count < 1)
            {
                SetCursorAtPosition(_head, true);
            }

            _head = Increment(_head);

            //Increase the count
            _count++;

            return true;
        }

        /// <summary>
        /// Removes the option selected by the cursor from the grid
        /// </summary>
        /// <returns>True if the option was removed successfully, otherwise returns false</returns>
        public bool Remove()
        {
            //Base Case
            if (_count < 1)
            {
                Debug.Log("[UIOptionsGrid] Tried to remove but the grid was empty");
                return false;
            }

            Destroy(_grid[_cursorPosition.x, _cursorPosition.y].gameObject);
            _grid[_cursorPosition.x, _cursorPosition.y] = null;
            _count--;

            if (_count < 1)
            {
                _cursorPosition = new IntVector2(0, 0);
                _head = new IntVector2(0, 0);
                return true;
            }


            int maxi = _gridHeight;
            int maxj = _gridWidth;
            if (_instantiationAxis == Axis.Vertical)
            {
                maxi = _gridWidth;
                maxj = _gridHeight;
            }

            Queue<UIOption> queue = new Queue<UIOption>();
            for (int i = 0; i < maxi; i++)
            {
                for (int j = 0; j < maxj; j++)
                {
                    int x = j;
                    int y = i;
                    if (_instantiationAxis == Axis.Vertical)
                    {
                        x = i;
                        y = j;
                    }

                    if (_grid[x, y] != null)
                    {
                        queue.Enqueue(_grid[x, y]);
                    }
                }
            }

            _grid = new UIOption[_gridWidth, _gridHeight];

            for (int i = 0; i < maxi; i++)
            {
                for (int j = 0; j < maxj; j++)
                {
                    int x = j;
                    int y = i;
                    if (_instantiationAxis == Axis.Vertical)
                    {
                        x = i;
                        y = j;
                    }

                    if (queue.Count > 0)
                    {
                        _grid[x, y] = queue.Dequeue();
                    }
                }
            }

            _head = Decrement(_head);

            if (_cursorPosition == _head)
            {
                _cursorPosition = Decrement(_cursorPosition);
            }

            SetCursorAtPosition(_cursorPosition, true);

            return true;
        }

        /// <summary>
        /// Clears all options and resets all option dependent data from the grid
        /// </summary>
        public void Clear() {
            //Remove all elements in the grid
            for (int i = 0; i < _gridWidth ; i++) 
                for (int j = 0; j < _gridHeight; j++)
                    if (_grid[i, j] != null)
                    {
                        Destroy(_grid[i, j].gameObject);
                    }

            //Reset all private members that are dependent on grid elements
            _count = 0;
            _cursorPosition = new IntVector2(0, 0); 
            _head = new IntVector2(0, 0);
        }

        public void SetCursorAtHead()
        {
            foreach (UIOption option in _grid)
            {
                option.SetCursor(false);
            }

            _grid[0, 0].SetCursor(true);
        }
        #endregion

        #region Private Methods
        private bool Add(UIOption option)
        {
            //Base case
            if (_count + 1 > Size)
            {
                Debug.Log("[UIOptionsGrid] Tried to add but the grid was full");
                return false;
            }

            //Create and set up the added element
            _grid[_head.x, _head.y] = option;

            //If the cursor did not already exist, enable it
            if (_count < 1)
            {
                SetCursorAtPosition(_head, true);
            }

            _head = Increment(_head);

            //Increase the count
            _count++;

            return true;
        }

        private void ConfirmListener()
        {
            if (InputManager.ConfirmPress)
            {
                if (_count > 0)
                {
                    _grid[_cursorPosition.x, _cursorPosition.y].Confirm();
                    PlayAudioSource(_confirmAudioSource);
                }
                else
                {
                    Debug.Log("[UIOptionsGrid] Tried to select an option but the grid was empty");
                }
            }
        }

        private void NavigationListener(bool decrementInput, bool incrementInput, Axis axis, int size) {
            if (decrementInput || incrementInput)
            {
                int tempField;

                switch (axis)
                {
                    case Axis.Horizontal:
                        tempField = _cursorPosition.x;
                        break;
                    case Axis.Vertical:
                        tempField = _cursorPosition.y;
                        break;
                    default:
                        Debug.Log("[UIOptionsGrid] Could not determine the specified axis in the input listener");
                        return;
                }

                //Left or Up
                if (decrementInput)
                {
                    if (tempField == 0)
                    {
                        tempField = size - 1;
                    } else
                    {
                        tempField -= 1;
                    }
                }

                //Down or Right
                if (incrementInput)
                {
                    if (tempField == size - 1)
                    {
                        tempField = 0;
                    } else
                    {
                        tempField += 1;
                    }
                }

                switch(axis)
                {
                    case Axis.Horizontal:
                        if (_grid[tempField, _cursorPosition.y] == null)
                        {
                            PlayAudioSource(_errorAudioSource);
                            return;
                        }
                        break;
                    case Axis.Vertical:
                        if (_grid[_cursorPosition.x, tempField] == null)
                        {
                            PlayAudioSource(_errorAudioSource);
                            return;
                        }
                        break;
                    default:
                        Debug.Log("[UIOptionsGrid] Could not determine the specified axis in the input listener");
                        return;
                }

                SetCursorAtPosition(_cursorPosition, false);

                switch (axis)
                {
                    case Axis.Horizontal:
                        _cursorPosition.x = tempField;
                        break;
                    case Axis.Vertical:
                        _cursorPosition.y = tempField;
                        break;
                    default:
                        Debug.Log("[UIOptionsGrid] Could not determine the specified axis in the input listener");
                        return;
                }

                SetCursorAtPosition(_cursorPosition, true);

                PlayAudioSource(_navigateAudioSource);
            }
        }

        private IntVector2 Increment(IntVector2 vector)
        {
            IntVector2 v = vector;
            switch (_instantiationAxis)
            {
                case Axis.Horizontal:
                    if (v.x + 1 >= _gridWidth)
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
                    if (v.y + 1 >= _gridHeight)
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
            switch (_instantiationAxis)
            {
                case Axis.Horizontal:
                    if (v.x - 1 < 0)
                    {
                        v.x = _gridWidth - 1;
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
                        v.y = _gridHeight - 1;
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
            _grid[cursorPosition.x, cursorPosition.y].SetCursor(value);
        }
        #endregion
    }
}
