using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Combat
{
    public class CombatGridTile : MonoBehaviour
    {
        public CharacterCombat AssignedCharacter { get { return this.assignedCharacter; } }
        private CharacterCombat assignedCharacter = null;

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
    }
}
