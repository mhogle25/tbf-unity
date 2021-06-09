using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.UI {
    public struct UIOptionData {
        public string text;
        public Sprite sprite;
        public Action action;
    }

    public class UIOptionsGroup : MonoBehaviour {
        [SerializeField] private UIOption optionPrefab;
        [Header("Direction of control")]
        [SerializeField] private bool horizontal;
        [SerializeField] private bool vertical;


        private UIOption[][] options;
        private int optionsWidth;
        private int optionsHeight;
        private Action action;
        private Vector2 position = new Vector2(0, 0);
        private Vector2 previousPosition = new Vector2(0, 0);

        private void Start() {
            action += () => {
                options[(int)position.x][(int)position.y].SetCursor(true);
                if (previousPosition != position) {
                    options[(int)previousPosition.x][(int)previousPosition.y].SetCursor(false);
                    previousPosition = position;
                }
            };

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

        public void Setup(UIOptionData[][] optionDatas, int width, int height) {
            Clear();

            optionsWidth = width;
            optionsHeight = height;

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
                position.x += InputManager.HorizontalAxis;
        }

        private void VerticalListener() {  
            if (InputManager.UpPress || InputManager.DownPress)
                position.y += InputManager.VerticalAxis;
        }
    }
}
