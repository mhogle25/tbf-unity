using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BF2D.Enums;

namespace BF2D.UI {

    public class UIOption : GridOption
    {
        [Tooltip("Reference to the Image component of the option (optional)")]
        [SerializeField] private Image icon = null;
        [Tooltip("Reference to the Image component of the option's cursor (required)")]
        [SerializeField] private Image cursor = null;
        [Tooltip("Reference to the TextMeshPro component of the option (optional)")]
        [SerializeField] private TextMeshProUGUI textMesh = null;

        /// <summary>
        /// Sets up an instantiated option
        /// </summary>
        /// <param name="data">The data in the option</param>
        public override void Setup(Data data)
        {
            base.Setup(data);

            if (!string.IsNullOrEmpty(data.text))
            {
                this.textMesh.gameObject.SetActive(true);
                this.textMesh.enabled = true;
                this.textMesh.text = data.text;
            }

            if (data.icon)
            {
                this.icon.gameObject.SetActive(true);
                this.icon.enabled = true;
                this.icon.sprite = data.icon;
            }
        }

        public override void SetCursor(bool status)
        {
            this.cursor.enabled = status;
        }
    }
}