using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.UI {
    public struct UIOptionData {
        public string text;
        public Sprite sprite;
        public Action action;
    };

    public class UIOptionsGroup : MonoBehaviour {

        private struct CursorPosition
        {
            public int x;
            public int y;

            public CursorPosition(int valueX, int valueY)
            {
                x = valueX;
                y = valueY;
            }

            public static bool operator ==(CursorPosition a, CursorPosition b)
            {
                return (a.x == b.x && a.y == b.y);
            }

            public static bool operator !=(CursorPosition a, CursorPosition b)
            {
                return !(a.x == b.x && a.y == b.y);
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [SerializeField] private UIOption optionPrefab;
        [Header("Direction of control")]
        [SerializeField] private bool horizontal;
        [SerializeField] private bool vertical;


        private UIOption[][] options;
        private int optionsWidth;
        private int optionsHeight;
        private Action action;
        private CursorPosition cursorPosition = new CursorPosition(0, 0);

        private void Start() {
            if (horizontal) {
                action += HorizontalListener;
            }

            if (vertical) {
                action += VerticalListener;
            }
        }

        private void Update() {
            action();
        }

        /// <summary>
        /// Need to make this populate with origin being in top left hand corner
        /// </summary>
        /// <param name="optionDatas"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Setup(UIOptionData[][] optionDatas, int width, int height) {
            Clear();

            optionsWidth = width;
            optionsHeight = height;

            //Idk if this populates correctly
            for (int i = 0, j = 0; i < optionsWidth && j < optionsHeight; i++, j++) {
                UIOption option = Instantiate(optionPrefab);
                options[i][j] = option;
                option.transform.parent = transform;
                option.transform.localScale = Vector3.one;
                option.Setup(optionDatas[i][j]);
            }
        }

        public void Clear() {
            for (int i = 0, j = 0; i < optionsWidth && j < optionsHeight; i++, j++) {
                Destroy(options[i][j].gameObject);
            }
        }

        private void HorizontalListener() {
            if (InputManager.LeftPress || InputManager.RightPress)
            {
                SetCursorAtCoordinates(cursorPosition, false);

                if (InputManager.LeftPress)
                {
                    if (cursorPosition.x == 0)
                    {
                        cursorPosition.x = optionsWidth - 1;
                    } else
                    {
                        cursorPosition.x -= 1;
                    }
                }

                if (InputManager.RightPress)
                {
                    if (cursorPosition.x == optionsWidth - 1)
                    {
                        cursorPosition.x = 0;
                    } else
                    {
                        cursorPosition.x += 1;
                    }
                }

                SetCursorAtCoordinates(cursorPosition, true);
            }
        }

        private void VerticalListener() {
            if (InputManager.UpPress || InputManager.DownPress)
            {
                SetCursorAtCoordinates(cursorPosition, false);

                if (InputManager.UpPress)
                {
                    if (cursorPosition.y == 0)
                    {
                        cursorPosition.y = optionsHeight - 1;
                    } else
                    {
                        cursorPosition.y -= 1;
                    }

                }

                if (InputManager.DownPress)
                {
                    if (cursorPosition.y == optionsHeight - 1)
                    {
                        cursorPosition.y = 0;
                    } else
                    {
                        cursorPosition.y += 1;
                    }

                }

                SetCursorAtCoordinates(cursorPosition, true);
            }
        }

        private void SetCursorAtCoordinates(CursorPosition cursorPosition, bool value)
        {
            options[cursorPosition.x][cursorPosition.y].SetCursor(value);
        }
    }
}
