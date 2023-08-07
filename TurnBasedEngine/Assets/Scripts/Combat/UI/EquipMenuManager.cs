using UnityEngine;
using BF2D.UI;
using UnityEngine.Events;

namespace BF2D.Game.Combat
{
    public class EquipMenuManager : OptionsGridControl
    {
        [Header("Platforms")]
        [SerializeField] private UnityEvent platformsOnConfirm = new();
        [Header("Equipped")]
        [SerializeField] private EquippedListControl equipped = null;
        [Header("Bag")]
        [SerializeField] private EquipmentBagControl equipmentBag = null;

        public override void ControlInitialize()
        {
            base.ControlInitialize();
            this.equipped.GridInitialize();
            PlatformsOnNavigate(new OptionsGrid.NavigateInfo
            {
                cursorPosition = this.Controlled.CursorPosition
            });
        }

        public override void ControlFinalize()
        {
            if (this.Controlled)
            {
                this.Controlled.SetCursorAtPosition(this.Controlled.CursorPosition, false);
                base.ControlFinalize();
            }
        }

        public void PlatformOnConfirm()
        {
            if (UICtx.One.IsControlling(this))
                this.platformsOnConfirm?.Invoke();
        }

        public void PlatformsOnNavigate(OptionsGrid.NavigateInfo info)
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

            this.equipped.SetupEquippedList(tile.AssignedCharacter.Stats);
        }

        public void EquippedOnNavigate(OptionsGrid.NavigateInfo info)
        {

        }
    }
}
