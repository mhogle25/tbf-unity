using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.UI
{
    public class UIOptionsGrid : MonoBehaviour {
        public enum Axis
        {
            Horizontal, Vertical
        };

        [SerializeField] private UIOption _optionPrefab;
        [SerializeField] private RectTransform _container;
        [SerializeField] private Axis _instantiationAxis;    //Determines the direction that elements will be populated in
        [SerializeField] private int _optionsWidth = 0;
        [SerializeField] private int _optionsHeight = 0;

        public int Size { get { return _optionsWidth * _optionsHeight; } }
        public int Count { get { return _count; } }

        private UIOption[,] _options;
        private int _count = 0;
        private IntVector2 _cursorPosition = new IntVector2(0, 0);
        private IntVector2 _head = new IntVector2(0, 0);

        private Action _action;

        private void Awake()
        {
            _action += () =>
            {
                //Horizontal Listener
                NavigationListener(InputManager.LeftPress, InputManager.RightPress, Axis.Horizontal, _optionsWidth);
                //Vertical Listener
                NavigationListener(InputManager.UpPress, InputManager.DownPress, Axis.Vertical, _optionsHeight);
            };

            _action += ConfirmListener;

            if (_optionsWidth > 0 && _optionsHeight > 0)
            {
                //Create the element data structure
                _options = new UIOption[_optionsWidth, _optionsHeight];
            }

            UIOption[] options = GetComponentsInChildren<UIOption>();

            if (options != null)
            {
                if (options.Length > 0)
                {
                    foreach (UIOption option in options)
                    {
                        Add(option);
                    }
                }
            }
        }

        private void Update() {
            _action();
        }

        #region Public Methods
        public void Setup(int width, int height)
        {
            //Clean up anything that could be left over
            Clear();

            //Set the width and height
            _optionsWidth = width;
            _optionsHeight = height;

            //Create the element data structure
            _options = new UIOption[_optionsWidth, _optionsHeight];
        }

        public bool Add(UIOptionData optionData)
        {
            //Base case
            if (_count + 1 > Size)
            {
                Debug.Log("[UIOptionsGrid]: Tried to add but the grid was full");
                return false;
            }

            //Create and set up the added element
            UIOption option = Instantiate(_optionPrefab);
            _options[_head.x, _head.y] = option;
            option.transform.parent = transform;
            option.transform.localScale = Vector3.one;
            option.Setup(optionData);

            //If the cursor did not already exist, enable it
            if (_count < 1)
            {
                SetCursorAtPosition(_head, true);
            }

            Increment(ref _head);

            //Increase the count
            _count++;

            return true;
        }

        public bool Remove()
        {
            //Base Case
            if (_count < 1)
            {
                Debug.Log("[UIOptionsGrid]: Tried to remove but the grid was empty");
                return false;
            }

            Destroy(_options[_cursorPosition.x, _cursorPosition.y].gameObject);
            _count--;

            if (_count < 1)
            {
                _cursorPosition = new IntVector2(0, 0);
                _head = new IntVector2(0, 0);
                return true;
            }

            UIOption[,] temp = _options;
            _options = new UIOption[_optionsWidth, _optionsHeight];

            Queue<UIOption> queue = new Queue<UIOption>();
            for (int i = 0; i < _optionsWidth; i++)
            {
                for (int j = 0; j < _optionsHeight; j++)
                {
                    if (temp[i, j] != null)
                    {
                        queue.Enqueue(temp[i, j]);
                    }
                }
            }

            for (int i = 0; i < _optionsWidth; i++)
            {
                for (int j = 0; j < _optionsHeight; j++)
                {
                    _options[i, j] = queue.Dequeue();
                }
            }

            if (_cursorPosition.Tuple == (_optionsWidth - 1, _optionsHeight - 1))
            {
                Decrement(ref _cursorPosition);
            }

            SetCursorAtPosition(_cursorPosition, true);

            Decrement(ref _head);

            return true;
        }

        public void Clear() {
            //Remove all elements in the grid
            for (int i = 0; i < _optionsWidth ; i++) 
                for (int j = 0; j < _optionsHeight; j++)
                    if (_options[i, j] != null)
                    {
                        Destroy(_options[i, j].gameObject);
                    }

            //Reset all private members that are dependent on grid elements
            _count = 0;
            _cursorPosition = new IntVector2(0, 0); 
            _head = new IntVector2(0, 0);
        }
        #endregion

        #region Private Methods
        private bool Add(UIOption option)
        {
            //Base case
            if (_count + 1 > Size)
            {
                Debug.Log("[UIOptionsGrid]: Tried to add but the grid was full");
                return false;
            }

            //Create and set up the added element
            _options[_head.x, _head.y] = option;

            //If the cursor did not already exist, enable it
            if (_count < 1)
            {
                SetCursorAtPosition(_head, true);
            }

            Increment(ref _head);

            //Increase the count
            _count++;

            return true;
        }

        private void ConfirmListener()
        {
            if (InputManager.ConfirmPress)
            {
                _options[_cursorPosition.x, _cursorPosition.y].Confirm();
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
                        Debug.Log("[UIOptionsGrid]: Could not determine the specified axis in the input listener");
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
                        if (_options[tempField, _cursorPosition.y] == null)
                        {
                            return;
                        }
                        break;
                    case Axis.Vertical:
                        if (_options[_cursorPosition.x, tempField] == null)
                        {
                            return;
                        }
                        break;
                    default:
                        Debug.Log("[UIOptionsGrid]: Could not determine the specified axis in the input listener");
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
                        Debug.Log("[UIOptionsGrid]: Could not determine the specified axis in the input listener");
                        return;
                }

                SetCursorAtPosition(_cursorPosition, true);
            }
        }

        private void Increment(ref IntVector2 vector)
        {
            switch (_instantiationAxis)
            {
                case Axis.Horizontal:
                    if (vector.x + 1 >= _optionsWidth)
                    {
                        vector.x = 0;
                        vector.y++;
                    }
                    else
                    {
                        vector.x++;
                    }
                    break;
                case Axis.Vertical:
                    if (vector.y + 1 >= _optionsHeight)
                    {
                        vector.y = 0;
                        vector.x++;
                    }
                    else
                    {
                        vector.y++;
                    }
                    break;
            }
        }

        private void Decrement(ref IntVector2 vector)
        {
            switch (_instantiationAxis)
            {
                case Axis.Horizontal:
                    if (vector.x - 1 < 1)
                    {
                        vector.x = _optionsWidth - 1;
                        vector.y--;
                    }
                    else
                    {
                        vector.x--;
                    }
                    break;
                case Axis.Vertical:
                    if (vector.y - 1 < 1)
                    {
                        vector.y = _optionsHeight - 1;
                        vector.x--;
                    }
                    else
                    {
                        vector.y--;
                    }
                    break;
            }
        }

        private void SetCursorAtPosition(IntVector2 cursorPosition, bool value)
        {
            _options[cursorPosition.x, cursorPosition.y].SetCursor(value);
        }
        #endregion
    }
}
