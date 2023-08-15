using UnityEngine;
using BF2D.UI;
using UnityEngine.Events;

namespace BF2D.Game.Combat
{
    public class EquipPlayerTargeter : OptionsGridControl
    {
        [Header("Platforms")]
        [SerializeField] private UnityEvent platformsOnConfirm = new();
        [Header("Equipped")]
        [SerializeField] private EquippedListControl equipped = null;

        public CharacterStats Selected => this.selectedCharacter;
        private CharacterStats selectedCharacter = null;

        public override void ControlInitialize()
        {
            base.ControlInitialize();
            this.equipped.GridInitialize();
            this.Controlled.OnNavigate();
        }

        public override void ControlFinalize()
        {
            this.Controlled.SetCursorAtPosition(this.Controlled.CursorPosition, false);
            base.ControlFinalize();
        }

        public void OnConfirm()
        {
            if (UICtx.One.IsControlling(this))
                this.platformsOnConfirm?.Invoke();
        }

        public void OnNavigate(OptionsGrid.NavInfo info)
        {
            if (!UICtx.One.IsControlling(this))
                return;

            GridOption selectedPlatform = this.Controlled.At(info.cursorPosition);
            CombatGridTile tile = selectedPlatform as CombatGridTile;

            if (!tile)
            {
                Debug.LogError($"[EquipMenuControl:PlatformsOnNavigate] Grid option in player platforms was not a tile -> {selectedPlatform.name}");
                return;
            }

            this.selectedCharacter = tile.AssignedCharacter.Stats;
            this.equipped.Setup(this.selectedCharacter);
        }
    }
}
