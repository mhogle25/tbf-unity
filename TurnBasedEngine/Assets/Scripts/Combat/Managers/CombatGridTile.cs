using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Enums;

namespace BF2D.Combat
{
    public class CombatGridTile : GridOption
    {
        [SerializeField] private SpriteRenderer cursor = null;
        public CharacterCombat AssignedCharacter { get { return this.assignedCharacter; } }
        [SerializeField] private CharacterCombat assignedCharacter = null;

        public override bool Setup(Data optionData)
        {
            Debug.LogError($"[CombatGridTile:Setup] Setup should not be called on a static grid option");
            throw new System.NotImplementedException();
        }

        public override void SetCursor(bool status)
        {
            this.cursor.enabled = status;
        }

        public void AssignCharacter(CharacterCombat characterCombat)
        {
            this.assignedCharacter = characterCombat;

            characterCombat.transform.SetParent(this.transform);
            characterCombat.transform.localPosition = Vector3.zero;
            characterCombat.transform.localScale = Vector3.one;
        }

        public void ResetTile()
        {
            this.assignedCharacter = null;
        }

        public void TargetAssignedCharacter()
        {
            CombatManager.Instance.CharacterTargeter.SetTargets(new List<CharacterCombat>{ this.assignedCharacter });
        }
    }
}
