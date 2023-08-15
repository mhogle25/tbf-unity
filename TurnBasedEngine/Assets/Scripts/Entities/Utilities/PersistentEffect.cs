using Newtonsoft.Json;
using BF2D.Enums;
using BF2D.Game.Actions;
using BF2D.Game.Enums;
using System;
using UnityEngine;

namespace BF2D.Game
{
    public abstract class PersistentEffect : Entity, ICombatAligned
    {
        [JsonIgnore] public override string ID { get => this.id; set => this.id = value; }
        [JsonIgnore] private string id = string.Empty;

        [JsonIgnore] public virtual int SpeedModifier => this.speedModifier;
        [JsonIgnore] public virtual int AttackModifier => this.attackModifier;
        [JsonIgnore] public virtual int DefenseModifier => this.defenseModifier;
        [JsonIgnore] public virtual int FocusModifier => this.focusModifier; 
        [JsonIgnore] public virtual int LuckModifier => this.luckModifier; 
        [JsonIgnore] public virtual int MaxHealthModifier => this.maxHealthModifier; 
        [JsonIgnore] public virtual int MaxStaminaModifier => this.maxStaminaModifier; 
        [JsonIgnore] public virtual UntargetedGameAction OnUpkeep => this.onUpkeep; 
        [JsonIgnore] public virtual UntargetedGameAction OnEOT => this.onEOT;

        [JsonIgnore] public virtual CombatAlignment Alignment
        {
            get
            {
                if (this.alignment is not null)
                    return this.alignment.GetValueOrDefault();

                int offensePoints = 0;
                int defensePoints = 0;
                int neutralPoints = 0;

                neutralPoints += this.SpeedModifier;
                offensePoints += this.AttackModifier;
                defensePoints += this.DefenseModifier;
                neutralPoints += this.FocusModifier;
                neutralPoints += this.LuckModifier;
                defensePoints += this.MaxHealthModifier;
                neutralPoints += this.MaxStaminaModifier;

                return CombatAlignmentSelector.Calculate(offensePoints, defensePoints, neutralPoints);
            }
        }

        [JsonProperty] private readonly CombatAlignment? alignment = null;
        [JsonProperty] private readonly int speedModifier = 0;
        [JsonProperty] private readonly int attackModifier = 0;
        [JsonProperty] private readonly int defenseModifier = 0;
        [JsonProperty] private readonly int focusModifier = 0;
        [JsonProperty] private readonly int luckModifier = 0;
        [JsonProperty] private readonly int maxHealthModifier = 0;
        [JsonProperty] private readonly int maxStaminaModifier = 0;
        [JsonProperty] private readonly UntargetedGameAction onUpkeep = null;
        [JsonProperty] private readonly UntargetedGameAction onEOT = null;

        public bool UpkeepEventExists() => this.onUpkeep is not null;

        public bool EOTEventExists() => this.onEOT is not null;

        public int GetModifier(CharacterStatsProperty property) => property switch
        {
            CharacterStatsProperty.Speed => this.SpeedModifier,
            CharacterStatsProperty.Attack => this.AttackModifier,
            CharacterStatsProperty.Defense => this.DefenseModifier,
            CharacterStatsProperty.Focus => this.FocusModifier,
            CharacterStatsProperty.Luck => this.LuckModifier,
            CharacterStatsProperty.MaxHealth => this.MaxHealthModifier,
            CharacterStatsProperty.MaxStamina => this.MaxStaminaModifier,
            _ => throw new ArgumentException($"[PeristentEffect:GetModifier] The given CharacterStatsProperty was null or invalid")
        };

        public string TextBreakdown() => TextBreakdown(null);

        public string TextBreakdown(CharacterStats source) => TextBreakdown(null, source);

        public virtual string TextBreakdown(Equipment other, CharacterStats source)
        {
            string text = $"{this.Name}\n-\n{this.Description}\n-\n";

            text += TbModifier(this.SpeedModifier, Strings.Character.SPEED, Strings.Character.SPEED_SYMBOL, other?.SpeedModifier);
            text += TbModifier(this.AttackModifier, Strings.Character.ATTACK, Strings.Character.ATTACK_SYMBOL, other?.AttackModifier);
            text += TbModifier(this.DefenseModifier, Strings.Character.DEFENSE, Strings.Character.DEFENSE_SYMBOL, other?.DefenseModifier);
            text += TbModifier(this.FocusModifier, Strings.Character.FOCUS, Strings.Character.FOCUS_SYMBOL, other?.FocusModifier);
            text += TbModifier(this.LuckModifier, Strings.Character.LUCK, Strings.Character.LUCK_SYMBOL, other?.LuckModifier);
            text += TbModifier(this.MaxHealthModifier, Strings.Character.MAX_HEALTH, Strings.Character.MAX_HEALTH_SYMBOL, other?.MaxHealthModifier);
            text += TbModifier(this.MaxStaminaModifier, Strings.Character.MAX_STAMINA, Strings.Character.MAX_STAMINA_SYMBOL, other?.MaxStaminaModifier);

            text += TbGameAction(this.OnUpkeep, "On Begin", source);
            text += TbGameAction(this.OnEOT, "On End", source);

            return text;
        }

        private string TbModifier(int modifier, string label, char icon, int? other)
        {
            if (modifier == 0)
                return string.Empty;

            int resolvedOther = other is null ? modifier : other.GetValueOrDefault();

            string sign = modifier > 0 ? "+" : modifier < 0 ? "-" : null;

            string relativeSign;
            Color32? color;
            if (modifier > resolvedOther)
            {
                relativeSign = $"{Strings.System.UP_ARROW_SYMBOL}";
                color = Colors.Green;
            }
            else if (modifier < resolvedOther)
            {
                relativeSign = $"{Strings.System.DOWN_ARROW_SYMBOL}";
                color = Colors.Red;
            }
            else
            {
                relativeSign = null;
                color = null;
            }

            string colorOpeningTag = color is not null ? $"<color=#{ColorUtility.ToHtmlStringRGBA(color.GetValueOrDefault())}>" : null;
            string colorClosingTag = color is not null ? "</color>" : null;

            return $"{icon} {colorOpeningTag}{sign}{modifier}{relativeSign} {label}{colorClosingTag}\n";
        }

        private string TbGameAction(UntargetedGameAction gameAction, string label, CharacterStats source)
        {
            if (gameAction is null)
                return string.Empty;

            return $"{gameAction.TextBreakdown(source)}{label}\n";
        }
    }
}