using System;
using Newtonsoft.Json;
using BF2D.Game.Combat.Enums;
using BF2D.Game.Enums;
using System.Collections.Generic;
using BF2D.Game.Actions;
using UnityEngine;

namespace BF2D.Game.Combat
{
    [Serializable]
    public class CharacterCombatAI
    {
        private const int REROLL_LIMIT = 20;

        [JsonIgnore] public bool Enabled { get => this.enabled; }
        [JsonIgnore] private bool enabled = false;

        [Serializable]
        private class Ranking<T> : Dictionary<T, int>
        {
            [JsonIgnore] public T Max
            {
                get
                {
                    if (this.Count < 1)
                        return default;

                    KeyValuePair<T, int> max = new(default, 0);
                    foreach (KeyValuePair<T, int> actionRanking in this)
                    {
                        if (actionRanking.Value > max.Value)
                            max = actionRanking;
                    }

                    return max.Key;
                }
            }

            public T Roll()
            {
                List<T> actionPool = new();
                int total = 0;
                foreach (KeyValuePair<T, int> actionRanking in this)
                {
                    total += actionRanking.Value;
                    for (int i = 0; i < actionRanking.Value; i++)
                        actionPool.Add(actionRanking.Key);
                }

                int random = UnityEngine.Random.Range(0, total);
                return actionPool[random];
            }
        }

        [JsonProperty] private readonly Ranking<CombatActionType> actionRanking = new();
        [JsonProperty] private readonly Ranking<AuraType> auraRanking = new();
        [JsonProperty] private readonly Ranking<CombatAlignment> alignmentRanking = new();

        private bool ChaoticCurrentCharacter => CombatCtx.One.CurrentCharacter.Stats.Chaotic;

        public void Run()
        {
            bool RunAction(CombatActionType action) => action switch
            {
                CombatActionType.Act => throw new NotImplementedException(),
                CombatActionType.Equip => TryRollEquipAction(),
                CombatActionType.Item => TryRollItemAction(),
                CombatActionType.Event => throw new NotImplementedException(),
                CombatActionType.Flee => throw new NotImplementedException(),
                CombatActionType.Roster => throw new NotImplementedException(),
                _ => false
            };

            this.enabled = true;
            int i = 0;

            while (i < CharacterCombatAI.REROLL_LIMIT)
            {
                CombatActionType chosenAction = RollActionType();

                if (RunAction(chosenAction))
                    break;
                else
                    i++;
            }

            if (i >= CharacterCombatAI.REROLL_LIMIT)
                TryRollItemAction(); // TODO: Change this to a default CombatActionType (once one is created, like a DEFEND or a basic ATTACK)

        }

        public void SetupTargetedGems()
        {
            IEnumerable<TargetedCharacterActionSlot> slots = CombatCtx.One.CurrentCharacter.CurrentCombatAction.GetTargetedGems();

            foreach (TargetedCharacterActionSlot slot in slots)
            {
                switch (slot.Target)
                {
                    case CharacterTarget.Self:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.CurrentCharacter };
                        break;
                    case CharacterTarget.Ally:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAnAlly(slot) };
                        break;
                    case CharacterTarget.AllAllies:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.Allies;
                        break;
                    case CharacterTarget.Opponent:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAnOpponent(slot) };
                        break;
                    case CharacterTarget.AllOpponents:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.Opponents;
                        break;
                    case CharacterTarget.Any:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickACharacter(slot) };
                        break;
                    case CharacterTarget.AllOfAny:
                        slot.TargetInfo.CombatTargets = slot.Alignment == CombatAlignment.Offense ||
                        slot.Alignment == CombatAlignment.Neutral ?
                        CombatCtx.One.Opponents :
                        CombatCtx.One.Allies;

                        if (this.ChaoticCurrentCharacter)
                            slot.TargetInfo.CombatTargets = UnityEngine.Random.Range(0, 2) == 0 ?
                            CombatCtx.One.Opponents :
                            CombatCtx.One.Allies;
                        break;
                    case CharacterTarget.All:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.Characters;
                        break;
                    case CharacterTarget.Random:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomCharacter() };
                        break;
                    case CharacterTarget.RandomAlly:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomAlly() };
                        break;
                    case CharacterTarget.RandomOpponent:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomOpponent() };
                        break;
                    default:
                        Debug.LogError("[CharacterCombatAI:SetupTargetedGems] The provided value for a character target was invalid");
                        break;
                }
            }

            CombatCtx.One.RunCombatEvents();
            this.enabled = false;
        }

        /// <returns>False if caller should roll for another action, otherwise true</returns>
        private bool TryRollItemAction()
        {
            CharacterStats character = CombatCtx.One.CurrentCharacter.Stats;

            if (character.ItemsCount < 1)
                return false;

            IEntityInfo info = RollForUtility(character.Items.Useable);

            if (info is null)
                return false;

            ItemInfo item = info as ItemInfo;
            CombatCtx.One.SetupItemCombat(item);
            return true;
        }

        /// <returns>False if caller should roll for another action, otherwise true</returns>
        private bool TryRollEquipAction()
        {
            CharacterStats character = CombatCtx.One.CurrentCharacter.Stats;

            if (character.EquipmentCount < 1)
                return false;

            IEntityInfo info = RollForUtility(character.Equipment);

            if (info is null)
                return false;

            EquipmentInfo equipment = info as EquipmentInfo;
            CombatCtx.One.SetupEquipCombat(equipment, equipment.Type);
            return true;
        }

        private CombatActionType RollActionType()
        {
            CombatActionType result;

            do result = this.actionRanking.Roll();
            while (result == CombatActionType.Equip && CombatCtx.One.CurrentCharacter.Stats.EquipmentCount < 1);

            return result;
        }

        private IEntityInfo RollForUtility(IEnumerable<UtilityEntityInfo> entities)
        {
            AuraType aura = this.auraRanking.Roll();
            CombatAlignment alignment = this.alignmentRanking.Roll();

            List<UtilityEntityInfo> list = new();

            bool allRestoration = true;
            foreach (UtilityEntityInfo entity in entities)
            {
                if (!entity.IsRestoration)
                    allRestoration = false;
                if (entity.Alignment == alignment)
                    list.Add(entity);
            }

            foreach(UtilityEntityInfo entity in entities)
                if (entity.ContainsAura(aura))
                    list.Add(entity);

            if (list.Count < 1)
                foreach (UtilityEntityInfo entity in entities)
                    if (entity.Alignment == this.alignmentRanking.Max)
                        list.Add(entity);

            if (list.Count < 1)
                foreach (UtilityEntityInfo entity in entities)
                    if (entity.ContainsAura(this.auraRanking.Max))
                        list.Add(entity);

            if (CombatCtx.One.AlliesAreAtFullHealth() && !allRestoration)
            {
                list.RemoveAll((entity) => entity.IsRestoration);

                if (list.Count < 1)
                    foreach (UtilityEntityInfo entity in entities)
                        if (!entity.IsRestoration)
                            list.Add(entity);
            }

            if (list.Count < 1)
                return null;

            int random = UnityEngine.Random.Range(0, list.Count);
            return list[random];
        }

        private CharacterCombat PickAnAlly(TargetedCharacterActionSlot gem)
        {
            if (HealCheck(gem))
            {
                List<CharacterCombat> allies = new();
                foreach (CharacterCombat character in CombatCtx.One.Allies)
                    allies.Add(character);

                allies.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(allies);
            }

            return CombatCtx.One.RandomAlly();
        }

        private CharacterCombat PickAnOpponent(TargetedCharacterActionSlot gem)
        {
            // TODO: more complex opponent selecting?
            return CombatCtx.One.RandomOpponent();
        }

        private CharacterCombat PickACharacter(TargetedCharacterActionSlot gem)
        {
            if (HealCheck(gem))
            {
                List<CharacterCombat> characters = new();
                foreach (CharacterCombat character in CombatCtx.One.Allies)
                    characters.Add(character);

                characters.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(characters);
            }

            if (gem.Alignment == CombatAlignment.Defense && !this.ChaoticCurrentCharacter)
                return PickAnAlly(gem);

            if (gem.Alignment == CombatAlignment.Offense && !this.ChaoticCurrentCharacter)
                return PickAnOpponent(gem);

            return CombatCtx.One.RandomCharacter();
        }


        private CharacterCombat PickACharacter(List<CharacterCombat> characters)
        {
            CharacterCombat chosen = null;
            for (int i = 0; i < characters.Count; i++)
            {
                if (!characters[i].Stats.Dead)
                {
                    chosen = characters[i];
                    break;
                }
            }
            return chosen;
        }

        private bool HealCheck(TargetedCharacterActionSlot gem)
        {
            return gem.IsRestoration &&
                (gem.Alignment == CombatAlignment.Defense || gem.Alignment == CombatAlignment.Neutral) &&
                !this.ChaoticCurrentCharacter;
        }

    }
}