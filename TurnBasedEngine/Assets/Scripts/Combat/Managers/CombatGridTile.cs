using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using UnityEngine.UI;

namespace BF2D.Game.Combat
{
    public class CombatGridTile : GridOption
    {
        [Header("Settings")]
        [SerializeField] private Vector3 assignmentPosition = Vector3.zero;
        [Header("UI/Grid Elements")]
        [SerializeField] private SpriteRenderer cursor = null;
        [SerializeField] private Canvas displayCanvas = null;
        [SerializeField] private Slider healthBar = null;
        [SerializeField] private Slider staminaBar = null;
        public CharacterCombat AssignedCharacter { get { return this.assignedCharacter; } }
        [Header("Display")]
        [SerializeField] private CharacterCombat assignedCharacter = null;

        public override bool Interactable
        {
            get
            {
                return this.interactable && this.assignedCharacter != null && !this.assignedCharacter.Stats.Dead;
            }
        }

        public override bool Setup(Data optionData)
        {
            Terminal.IO.LogError($"[CombatGridTile:Setup] Setup should not be called on a static grid option");
            throw new System.NotImplementedException();
        }

        public override void SetCursor(bool status)
        {
            this.cursor.enabled = status;
        }

        public void SetHealthBar()
        {
            if (this.assignedCharacter == null)
                return;
            this.healthBar.value = ((float)this.assignedCharacter.Stats.Health) / this.assignedCharacter.Stats.MaxHealth;
        }

        public void SetStaminaBar()
        {
            if (this.assignedCharacter == null)
                return;
            this.staminaBar.value = ((float)this.assignedCharacter.Stats.Stamina) / this.assignedCharacter.Stats.MaxStamina;
        }

        public void AssignCharacter(CharacterCombat characterCombat)
        {
            this.assignedCharacter = characterCombat;

            characterCombat.transform.SetParent(this.transform);
            characterCombat.transform.localPosition = this.assignmentPosition;
            characterCombat.transform.localScale = Vector3.one;

            this.displayCanvas.enabled = true;
            SetHealthBar();
            SetStaminaBar();

            characterCombat.Tile = this;
        }

        public void ResetTile()
        {
            this.displayCanvas.enabled = false;
            this.assignedCharacter = null;
        }

        public void TargetAssignedCharacter()
        {
            CombatManager.Instance.CharacterTargeter.SetTargets(new List<CharacterCombat>{ assignedCharacter });
        }
    }
}
