using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using BF2D.Game.Actions;
using BF2D.Game.Enums;

namespace BF2D.Game
{
    [Serializable]
    public class EquipModSlot : ICombatAligned
    {
        [JsonProperty] private string runeID = string.Empty;

        [JsonIgnore] private EquipMod Rune => GameCtx.Instance.GetRune(this.runeID);

        [JsonIgnore] public bool Armed => this.Rune is not null;

        [JsonIgnore] public CombatAlignment Alignment => this.Rune.Alignment;

        [JsonIgnore] public virtual int SpeedModifier => this.Rune.SpeedModifier;
        [JsonIgnore] public virtual int AttackModifier => this.Rune.AttackModifier;
        [JsonIgnore] public virtual int DefenseModifier => this.Rune.DefenseModifier;
        [JsonIgnore] public virtual int FocusModifier => this.Rune.FocusModifier;
        [JsonIgnore] public virtual int LuckModifier => this.Rune.LuckModifier;
        [JsonIgnore] public virtual int MaxHealthModifier => this.Rune.MaxHealthModifier;
        [JsonIgnore] public virtual int MaxStaminaModifier => this.Rune.MaxStaminaModifier;
        [JsonIgnore] public virtual UntargetedGameAction OnUpkeep => this.Rune.OnUpkeep;
        [JsonIgnore] public virtual UntargetedGameAction OnEOT => this.Rune.OnEOT;

        public void SetRuneID(string id) => this.runeID = id;
    }
}
