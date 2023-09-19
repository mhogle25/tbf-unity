using System;
using Newtonsoft.Json;
using BF2D.Game.Combat.Enums;
using BF2D.Game.Enums;
using System.Collections.Generic;
using BF2D.Game.Actions;
using UnityEngine;
using System.Linq;

namespace BF2D.Game.Combat
{
    [Serializable]
    public class CharacterCombatAI
    {
        private const int REROLL_LIMIT = 20;

        [JsonIgnore] public bool Enabled => this.enabled;
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

        public void Run(CharacterCombat source)
        {
            bool RunAction(CombatActionType action) => action switch
            {
                CombatActionType.Act => throw new NotImplementedException(),
                CombatActionType.Equip => TryRollEquipAction(source),
                CombatActionType.Item => TryRollItemAction(source),
                CombatActionType.Event => throw new NotImplementedException(),
                CombatActionType.Flee => throw new NotImplementedException(),
                CombatActionType.Roster => throw new NotImplementedException(),
                _ => false
            };
            
            this.enabled = true;
            int i = 0;

            while (i < CharacterCombatAI.REROLL_LIMIT)
            {
                CombatActionType chosenAction = RollActionType(source);

                if (RunAction(chosenAction))
                    break;
                i++;
            }

            if (i >= CharacterCombatAI.REROLL_LIMIT)
                TryRollItemAction(source); // TODO: Change this to a default CombatActionType (once one is created, like a DEFEND or a basic ATTACK)
        }

        public void SetupTargetedGameAction(TargetedGameAction targetedGameAction, CharacterCombat source)
        {
            CharacterAlignment alignment = source.Alignment;
            IEnumerable<TargetedCharacterActionSlot> slots = targetedGameAction.TargetedGemSlots;
            
            foreach (TargetedCharacterActionSlot slot in slots)
            {
                switch (slot.Target)
                {
                    case CharacterTarget.Self:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { source };
                        break;
                    case CharacterTarget.Ally:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAnAlly(slot, source) };
                        break;
                    case CharacterTarget.AllAllies:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.GetAllies(alignment);
                        break;
                    case CharacterTarget.Opponent:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAnOpponent(slot, source) };
                        break;
                    case CharacterTarget.AllOpponents:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.GetOpponents(alignment);
                        break;
                    case CharacterTarget.Any:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickACharacter(slot, source) };
                        break;
                    case CharacterTarget.AllOfAny:
                        slot.TargetInfo.CombatTargets = slot.Alignment == CombatAlignment.Offense ||
                        slot.Alignment == CombatAlignment.Neutral ?
                        CombatCtx.One.GetOpponents(alignment) :
                        CombatCtx.One.GetAllies(alignment);

                        if (source.Stats.Chaotic)
                            slot.TargetInfo.CombatTargets = UnityEngine.Random.Range(0, 2) == 0 ?
                            CombatCtx.One.GetOpponents(alignment) :
                            CombatCtx.One.GetAllies(alignment);
                        break;
                    case CharacterTarget.All:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.Characters;
                        break;
                    case CharacterTarget.Random:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomCharacter() };
                        break;
                    case CharacterTarget.RandomAlly:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomAlly(alignment) };
                        break;
                    case CharacterTarget.RandomOpponent:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomOpponent(alignment) };
                        break;
                    default:
                        Debug.LogError("[CharacterCombatAI:SetupTargetedGems] The provided value for a character target was invalid");
                        break;
                }
            }

            this.enabled = false;
        }

        /// <returns>False if caller should roll for another action, otherwise true</returns>
        private bool TryRollItemAction(CharacterCombat source)
        {
            CharacterStats character = source.Stats;

            if (character.ItemsCount < 1)
                return false;

            IEntityInfo info = RollForUtility(character.Items.Useable, source);

            if (info is null)
                return false;

            ItemInfo item = info as ItemInfo;
            CombatCtx.One.SetupItemCombat(item);
            return true;
        }

        /// <returns>False if caller should roll for another action, otherwise true</returns>
        private bool TryRollEquipAction(CharacterCombat source)
        {
            CharacterStats character = source.Stats;

            if (character.EquipmentCount < 1)
                return false;

            IEntityInfo info = RollForUtility(character.Equipment, source);

            if (info is null)
                return false;

            EquipmentInfo equipment = info as EquipmentInfo;
            CombatCtx.One.SetupEquipCombat(equipment, equipment.Type);
            return true;
        }

        private CombatActionType RollActionType(CharacterCombat source)
        {
            CombatActionType result;

            do result = this.actionRanking.Roll();
            while (result == CombatActionType.Equip && source.Stats.EquipmentCount < 1);

            return result;
        }

        private IEntityInfo RollForUtility(IEnumerable<UtilityEntityInfo> utilityInfos, CharacterCombat source)
        {
            UtilityEntityInfo[] entityInfo = utilityInfos.ToArray();
            AuraType aura = this.auraRanking.Roll();
            CombatAlignment alignment = this.alignmentRanking.Roll();

            List<UtilityEntityInfo> list = new();

            bool allRestoration = true;
            foreach (UtilityEntityInfo entity in entityInfo)
            {
                if (!entity.IsRestoration)
                    allRestoration = false;
                if (entity.Alignment == alignment)
                    list.Add(entity);
            }

            foreach(UtilityEntityInfo entity in entityInfo)
                if (entity.ContainsAura(aura))
                    list.Add(entity);

            if (list.Count < 1)
                foreach (UtilityEntityInfo entity in entityInfo)
                    if (entity.Alignment == this.alignmentRanking.Max)
                        list.Add(entity);

            if (list.Count < 1)
                foreach (UtilityEntityInfo entity in entityInfo)
                    if (entity.ContainsAura(this.auraRanking.Max))
                        list.Add(entity);

            if (CombatCtx.One.AlliesAreAtFullHealth(source.Alignment) && !allRestoration)
            {
                list.RemoveAll(entity => entity.IsRestoration);

                if (list.Count < 1)
                    foreach (UtilityEntityInfo entity in entityInfo)
                        if (!entity.IsRestoration)
                            list.Add(entity);
            }

            if (list.Count < 1)
                return null;

            int random = UnityEngine.Random.Range(0, list.Count);
            return list[random];
        }

        private CharacterCombat PickAnAlly(TargetedCharacterActionSlot gem, CharacterCombat source)
        {
            if (HealCheck(gem, source))
            {
                List<CharacterCombat> allies = new();
                foreach (CharacterCombat character in CombatCtx.One.GetAllies(source.Alignment))
                    allies.Add(character);

                allies.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(allies);
            }

            return CombatCtx.One.RandomAlly(source.Alignment);
        }

        private CharacterCombat PickAnOpponent(TargetedCharacterActionSlot gem, CharacterCombat source)
        {
            // TODO: more complex opponent selecting?
            return CombatCtx.One.RandomOpponent(source.Alignment);
        }

        private CharacterCombat PickACharacter(TargetedCharacterActionSlot gem, CharacterCombat source)
        {
            if (HealCheck(gem, source))
            {
                List<CharacterCombat> characters = new();
                foreach (CharacterCombat character in CombatCtx.One.GetAllies(source.Alignment))
                    characters.Add(character);

                characters.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(characters);
            }

            if (gem.Alignment == CombatAlignment.Defense && !source.Stats.Chaotic)
                return PickAnAlly(gem, source);

            if (gem.Alignment == CombatAlignment.Offense && !source.Stats.Chaotic)
                return PickAnOpponent(gem, source);

            return CombatCtx.One.RandomCharacter();
        }


        private static CharacterCombat PickACharacter(List<CharacterCombat> characters)
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

        private static bool HealCheck(TargetedCharacterActionSlot gem, CharacterCombat source)
        {
            return gem.IsRestoration &&
                (gem.Alignment == CombatAlignment.Defense || gem.Alignment == CombatAlignment.Neutral) &&
                !source.Stats.Chaotic;
        }

    }
}