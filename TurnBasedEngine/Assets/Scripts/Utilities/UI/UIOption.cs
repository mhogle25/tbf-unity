using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BF2D.UI {
    public class UIOption : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] private Image image;
        [SerializeField] private Image cursor;

        public void Setup(UIOptionData optionData) {
            if (optionData.text != null) {
                textMesh.text = optionData.text;
            } else {
                textMesh.gameObject.SetActive(false);
            }

            if (optionData.sprite != null) {
                image.sprite = optionData.sprite;
            } else {
                image.gameObject.SetActive(false);
            }
        }

        public void SetCursor(bool status) {
            cursor.gameObject.SetActive(status);
        }
    }
}