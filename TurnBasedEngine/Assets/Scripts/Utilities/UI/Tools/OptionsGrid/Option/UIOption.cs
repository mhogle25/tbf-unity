﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using BF2D.Enums;
using BF2D.Attributes;

namespace BF2D.UI {

    public class UIOption : GridOption {
        [Tooltip("Reference to the Image component of the option (optional)")]
        [SerializeField] private Image icon = null;
        [Tooltip("Reference to the Image component of the option's cursor (required)")]
        [SerializeField] private Image cursor = null;
        [Tooltip("Reference to the TextMeshPro component of the option (optional)")]
        [SerializeField] private TextMeshProUGUI textMesh = null;

        /// <summary>
        /// Sets up an instantiated option
        /// </summary>
        /// <param iconID="optionData">The data in the option</param>
        /// <returns>True if setup is successful, otherwise returns false</returns>
        public override bool Setup(Data optionData)
        {
            this.gameObject.name = optionData.name;
            Data data = optionData;

            SetupText(data.text);
            SetupIcon(data.icon);

            foreach (InputButton inputButton in Enum.GetValues(typeof(InputButton)))
            {
                if (data.actions[inputButton] != null)
                {
                    GetInputEvent(inputButton).AddListener(() =>
                    {
                        data.actions[inputButton]();
                    });
                }
            }

            return true;
        }

        public override void SetCursor(bool status)
        {
            this.cursor.enabled = status;
        }

        private void SetupIcon(Sprite icon)
        {
            if (!this.icon)
                return;

            if (icon)
            {
                this.icon.sprite = icon;
            }
            else
            {
                if (this.icon.sprite == null)
                {
                    this.icon.enabled = false;
                }
            }
        }

        private void SetupText(string text)
        {
            if (!this.textMesh)
                return;

            if (text != null || text != string.Empty)
            {
                this.textMesh.text = text;
            }
            else
            {
                if (this.textMesh.text == null)
                {
                    this.textMesh.enabled = false;
                }
            }
        }
    }
}