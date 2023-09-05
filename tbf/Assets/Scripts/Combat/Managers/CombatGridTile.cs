using BF2D.UI;
using UnityEngine;
using UnityEngine.UI;
using BF2D.Game.Enums;

namespace BF2D.Game.Combat
{
    public class CombatGridTile : GridOption
    {
        [Header("Settings")]
        [SerializeField] private Vector3 assignmentPosition = Vector3.zero;
        [SerializeField] private CharacterAlignment alignment = CharacterAlignment.Player;
        [Header("UI/Grid Elements")]
        [SerializeField] private SpriteRenderer cursor = null;
        [SerializeField] private Canvas displayCanvas = null;
        [SerializeField] private Slider healthBar = null;
        [SerializeField] private Slider staminaBar = null;
        public CharacterCombat AssignedCharacter => this.assignedCharacter;
        [Header("VFX")]
        [SerializeField] private Material assignedCharacterMaterial;
        [Space(30)]
        [Header("Display")]
        [SerializeField] private CharacterCombat assignedCharacter = null;

        public override bool Interactable
        {
            get => this.interactable && this.assignedCharacter != null && !this.assignedCharacter.Stats.Dead;
        }

        public CharacterAlignment Alignment => this.alignment;

        public override sealed void Setup(Data data)
        {
            Debug.LogError($"[CombatGridTile:Setup] Setup should not be called on a static grid option");
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
            characterCombat.SetMaterial(this.assignedCharacterMaterial);

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

        public void ItemsEvent(ItemsCharacterTargeter targeter)
        {
            if (UICtx.One.IsControlling(targeter))
                targeter.SetSingleTarget(this.assignedCharacter);
        }
    }
}
