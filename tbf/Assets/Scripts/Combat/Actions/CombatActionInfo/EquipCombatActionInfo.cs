using System.Collections.Generic;
using System;
using BF2D.Game.Actions;
using BF2D.Game.Enums;

namespace BF2D.Game.Combat.Actions
{
    public class EquipCombatActionInfo
    {
        public EquipmentInfo Info { get => this.info; set => this.info = value; }
        private EquipmentInfo info = null;

        public EquipmentType Type { get => this.type; set => this.type = value; }
        private EquipmentType type = EquipmentType.Accessory;

        public UntargetedGameAction OnEquip => this.Info?.Get()?.OnEquip;

        public List<string> Run()
        {
            CharacterStats character = CombatCtx.One.CurrentCharacter.Stats;

            string message = string.Empty;

            EquipmentInfo removed;
            if (this.Info is null)
            {
                removed = character.Unequip(this.Type);
                message += $"{character.Name} unequipped {removed.Name}. {Strings.DialogTextbox.PAUSE_BREIF}{Strings.DialogTextbox.END}";
            }
            else
            {
                removed = character.Equip(this.Info);

                message += $"{character.Name} equipped {this.Info.Name}";

                if (removed is not null)
                    message += $" and put {removed.Name} away";
                message += $". {Strings.DialogTextbox.PAUSE_BREIF}{Strings.DialogTextbox.END}";
            }

            return new() { message };
        }
    }
}
