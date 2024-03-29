﻿using System;
using Newtonsoft.Json;
using BF2D.Game.Actions;
using BF2D.Game.Enums;
using UnityEngine;

namespace BF2D.Game
{
    [Serializable]
    public class Item : Entity, IUtilityEntity
    {
        [JsonIgnore] private string id = string.Empty;
        [JsonProperty] protected readonly string spriteID = string.Empty;
        [JsonProperty] protected readonly bool consumable = true;
        [JsonProperty] protected readonly TargetedGameAction onUse = null;

        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] public string SpriteID => this.spriteID;
        [JsonIgnore] public CombatAlignment Alignment => this.OnUse?.Alignment ?? CombatAlignment.Neutral;
        [JsonIgnore] public bool IsRestoration => this.OnUse?.IsRestoration ?? false;

        [JsonIgnore] public bool Consumable => this.consumable;
        [JsonIgnore] public TargetedGameAction OnUse => this.onUse;
        [JsonIgnore] public bool Useable => this.OnUse is not null; 
        [JsonIgnore] public bool CombatExclusive => this.Useable && this.OnUse.CombatExclusive;

        public Sprite GetIcon() => GameCtx.One.GetIcon(this.SpriteID);

        public string TextBreakdown(CharacterStats source)
        {
            string description = $"{base.Description}\n";

            description += this.OnUse.TextBreakdown(source);

            description += "-\n";
            if (!this.Consumable)
                description += "Non-Consumable\n-\n";

            return description;
        }
    }
}
