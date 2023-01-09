using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BF2D.Game;

namespace BF2D.Combat
{
    public class StatsPreview : MonoBehaviour
    {
        /*
        public bool AutoRefresh { get { return this.autoRefresh; } set { this.autoRefresh = value; } }
        [SerializeField] private bool autoRefresh = false;
        public string NameKey { get { return this.nameKey; } }
        [SerializeField] private string nameKey = string.Empty;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI nameTextGUI = null;
        [SerializeField] private TextMeshProUGUI healthTextGUI = null;
        [SerializeField] private Slider healthBar = null;
        [SerializeField] private TextMeshProUGUI staminaTextGUI = null;
        [SerializeField] private Slider staminaBar = null;
        [SerializeField] private Image cursor = null;

        public void Setup(string nameKey)
        {
            this.nameKey = nameKey;
        }

        private void Update()
        {
            if (this.autoRefresh)
            {
                //Refresh the UI every frame
                Refresh();
            }
        }

        public void Refresh()
        {
            if (this.nameKey == string.Empty)
            {
                Debug.LogWarning($"[StatsPreview] The iconID key for GameObject '{this.gameObject.iconID}' was empty");
                return;
            }

            //CharacterStats stats = GameInfo.Instance.GetPlayer(this.nameKey);

            if (stats == null)
            {
                Debug.LogWarning($"[StatsPreview] No player character with the iconID key '{this.nameKey}' was found in GlobalGameManager");
                return;
            }

            nameTextGUI.text = stats.PrefabID;
            healthTextGUI.text = $"{stats.Health}/{stats.MaxHealth}";
            healthBar.value = stats.Health / stats.MaxHealth;
            staminaTextGUI.text = $"{stats.Stamina}/{stats.MaxStamina}";
            staminaBar.value = stats.Stamina / stats.MaxStamina;
        }

        public void SetCursor(bool value)
        {
            cursor.enabled = value;
        }
        */
    }
}
