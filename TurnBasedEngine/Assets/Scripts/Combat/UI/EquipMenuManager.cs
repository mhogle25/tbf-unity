using System.Collections.Generic;
using UnityEngine;
using BF2D.UI;
using BF2D.Game.Enums;
using System;
using BF2D.Enums;
using UnityEngine.Events;

namespace BF2D.Game.Combat
{
    public class EquipMenuManager : OptionsGridControl
    {
        [Header("Platforms")]
        [SerializeField] private UnityEvent platformsOnConfirm = new();
        [Header("Equipped")]
        [SerializeField] private OptionsGridControl equipped = null;
        [SerializeField] private InputEvents equippedEvents = null;
        [Header("Bag")]
        [SerializeField] private EquipmentBagControl equipmentBag = null;

        private readonly List<EquipmentType> currentEquipped = new();

        public void PlatformOnConfirm()
        {
            if (UICtx.One.IsControlling(this))
                this.platformsOnConfirm?.Invoke();
        }

        public void PlatformsOnNavigate(OptionsGrid.NavigateInfo info)
        {
            if (!UICtx.One.IsControlling(this))
                return;

            GridOption option = this.controlled.At(info.cursorPosition);
            CombatGridTile tile = option as CombatGridTile;

            if (!tile)
            {
                Debug.LogError($"[EquipMenuControl:PlatformsOnNavigate] Grid option in player platforms was not a tile -> {option.name}");
                return;
            }

            SetupEquipped(tile.AssignedCharacter.Stats);
        }

        public void EquippedOnNavigate(OptionsGrid.NavigateInfo info)
        {
            if (!UICtx.One.IsControlling(this.equipped))
                return;

            EquipmentType type = this.currentEquipped[info.cursorPosition1D];
            IEnumerable<EquipmentInfo> equipments = CombatCtx.One.CurrentCharacter.Stats.Equipments.FilterByType(type);

            this.equipmentBag.LoadOptionsFromInfos(equipments);
        }

        private void SetupEquipped(CharacterStats stats)
        {
            OptionsGrid grid = this.equipped.Controlled;
            grid.Setup(grid.Width, grid.Height);

            if (this.currentEquipped.Count < 1)
                foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
                    this.currentEquipped.Add(type);

            foreach (EquipmentType type in Enum.GetValues(typeof(EquipmentType)))
            {
                Equipment equipment = stats.GetEquipped(type);

                GridOption.Data data = new()
                {
                    name = equipment?.Name,
                    icon = equipment?.GetIcon(),
                    actions = equipment is not null ? new InputButtonCollection<Action>
                    {
                        [InputButton.Confirm] = this.equippedEvents.ConfirmEvent.Invoke,
                        [InputButton.Back] = this.equippedEvents.BackEvent.Invoke
                    } : null
                };

                grid.Add(data);
            }
        }
    }
}
