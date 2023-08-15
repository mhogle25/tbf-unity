using System;
using Newtonsoft.Json;
using BF2D.Game.Combat.Enums;
using BF2D.Game.Enums;
using System.Collections.Generic;
using BF2D.Game.Actions;
using BF2D.Enums;
using UnityEngine;

namespace BF2D.Game.Combat
{
    [Serializable]
    public class CharacterCombatAI
    {
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

        public void Run() 
        {
            this.enabled = true;

            CombatActionType chosenAction = RollActionType();

            //Roll Action Info
            switch (chosenAction)
            {
                case CombatActionType.Act: break;   //TODO
                case CombatActionType.Equip: break; //TODO
                case CombatActionType.Event: break; //TODO
                case CombatActionType.Flee: break;  //TODO
                case CombatActionType.Item:
                    RollItemAction();
                    break;
                case CombatActionType.Roster: break;
            }
        }

        public void SetupTargetedGems()
        {
            IEnumerable<TargetedCharacterStatsActionSlot> slots = CombatCtx.One.CurrentCharacter.CurrentCombatAction.GetTargetedGemSlots();

            foreach (TargetedCharacterStatsActionSlot slot in slots)
            {
                switch (slot.Target)
                {
                    case CharacterTarget.Self:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.CurrentCharacter };
                        break;
                    case CharacterTarget.Ally:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAnEnemy(slot) };
                        break;
                    case CharacterTarget.AllAllies:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.Players;
                        break;
                    case CharacterTarget.Opponent:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAPlayer(slot) };
                        break;
                    case CharacterTarget.AllOpponents:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.Enemies;
                        break;
                    case CharacterTarget.Any:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { PickACharacter(slot) };
                        break;
                    case CharacterTarget.AllOfAny:
                        slot.TargetInfo.CombatTargets = slot.Alignment == CombatAlignment.Offense ||
                        slot.Alignment == CombatAlignment.Neutral ?
                        CombatCtx.One.Players :
                        CombatCtx.One.Enemies;

                        if (CurrentCharacterIsChaotic())
                            slot.TargetInfo.CombatTargets = UnityEngine.Random.Range(0, 2) == 0 ?
                            CombatCtx.One.Players :
                            CombatCtx.One.Enemies;
                        break;
                    case CharacterTarget.All:
                        slot.TargetInfo.CombatTargets = CombatCtx.One.Characters;
                        break;
                    case CharacterTarget.Random:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomCharacter() };
                        break;
                    case CharacterTarget.RandomAlly:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomEnemy() };
                        break;
                    case CharacterTarget.RandomOpponent:
                        slot.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatCtx.One.RandomPlayer() };
                        break;
                    default:
                        Debug.LogError("[CharacterCombatAI:SetupTargetedGems] The provided value for a character target was invalid");
                        break;
                }
            }

            CombatCtx.One.RunCombatEvents();
            this.enabled = false;
        }

        private void RollItemAction()
        {
            ItemInfo item = RollForUtility(CombatCtx.One.CurrentCharacter.Stats.Items.Useable) as ItemInfo;
            CombatCtx.One.SetupItemCombat(item);
        }

        private CombatActionType RollActionType()
        {
            return this.actionRanking.Roll();
        }

        private IEntityInfo RollForUtility(IEnumerable<UtilityEntityInfo> entities)
        {
            AuraType aura = this.auraRanking.Roll();
            CombatAlignment alignment = this.alignmentRanking.Roll();

            List<IEntityInfo> list = new();

            bool allRestoration = true;
            foreach (UtilityEntityInfo entity in entities)
            {
                if (!entity.ContainsAura(AuraType.Restoration))
                    allRestoration = false;
                if (entity.GetUtility().Alignment == alignment)
                    list.Add(entity);
            }

            foreach(IEntityInfo entity in entities)
                if (entity.ContainsAura(aura))
                    list.Add(entity);

            if (list.Count < 1)
                foreach (UtilityEntityInfo entity in entities)
                    if (entity.GetUtility().Alignment == this.alignmentRanking.Max)
                        list.Add(entity);

            if (list.Count < 1)
                foreach (IEntityInfo entity in entities)
                    if (entity.ContainsAura(this.auraRanking.Max))
                        list.Add(entity);

            if (CombatCtx.One.EnemiesAreAtFullHealth() && !allRestoration)
            {
                list.RemoveAll((entity) => entity.ContainsAura(AuraType.Restoration));

                if (list.Count < 1)
                    foreach (IEntityInfo entity in entities)
                        if (!entity.ContainsAura(AuraType.Restoration))
                            list.Add(entity);
            }
            else
            {
                if (list.Count < 1)
                    foreach (IEntityInfo entity in entities)
                        list.Add(entity);
            }

            int random = UnityEngine.Random.Range(0, list.Count);
            return list[random];
        }

        private CharacterCombat PickAnEnemy(TargetedCharacterStatsActionSlot gem)
        { 
            List<CharacterCombat> enemies = new();
            foreach (CharacterCombat enemy in CombatCtx.One.Enemies)
                enemies.Add(enemy);

            if (HealLogicCheck(gem))
            {
                enemies.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(enemies);
            }

            return enemies[UnityEngine.Random.Range(0, enemies.Count)];
        }

        private CharacterCombat PickAPlayer(TargetedCharacterStatsActionSlot gem)
        {
            List<CharacterCombat> players = new();
            foreach (CharacterCombat player in CombatCtx.One.Players)
                players.Add(player);

            return players[UnityEngine.Random.Range(0, players.Count)];
        }

        private CharacterCombat PickACharacter(TargetedCharacterStatsActionSlot gem)
        {
            List<CharacterCombat> characters = new();
            foreach (CharacterCombat character in CombatCtx.One.Enemies)
                characters.Add(character);

            if (HealLogicCheck(gem))
            {
                characters.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(characters);
            }

            foreach (CharacterCombat character in CombatCtx.One.Players)
                characters.Add(character);

            if (gem.Alignment == CombatAlignment.Defense && !CurrentCharacterIsChaotic())
                return PickAnEnemy(gem);

            if (gem.Alignment == CombatAlignment.Offense && !CurrentCharacterIsChaotic())
                return PickAPlayer(gem);

            return characters[UnityEngine.Random.Range(0, characters.Count)];
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

        private bool HealLogicCheck(TargetedCharacterStatsActionSlot gem)
        {
            return gem.ContainsAura(AuraType.Restoration) &&
                (gem.Alignment == CombatAlignment.Defense || gem.Alignment == CombatAlignment.Neutral) &&
                !CurrentCharacterIsChaotic();
        }

        private bool CurrentCharacterIsChaotic()
        {
            return CombatCtx.One.CurrentCharacter.Stats.ContainsAura(AuraType.Chaos);
        }
    }
}