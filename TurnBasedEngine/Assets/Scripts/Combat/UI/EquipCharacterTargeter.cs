using UnityEngine;
using BF2D.UI;
using UnityEngine.Events;

namespace BF2D.Game.Combat
{
    public class EquipCharacterTargeter : OptionsGridControl
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
            OnNavigate(this.Controlled.GetSnapshot());
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

        public void OnNavigate(OptionsGrid.Snapshot info)
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
            this.equipped.SetupEquippedList(this.selectedCharacter);
        }
    }
}
