using System;
using Newtonsoft.Json;
using BF2D.Combat.Enums;
using BF2D.Enums;
using BF2D.Game;
using System.Collections.Generic;
using BF2D.Game.Actions;

namespace BF2D.Combat
{
    [Serializable]
    public class CharacterCombatAI
    {
        [JsonIgnore] public bool Enabled { get { return this.enabled; } }
        [JsonIgnore] public bool enabled = false;

        [Serializable]
        private class Ranking<T> where T : Enum
        {
            private class RankingInfo<U>
            {
                [JsonIgnore] public U Value { get { return this.value; } }
                [JsonProperty] private readonly U value = default;
                [JsonIgnore] public int Weight { get { return this.weight; } }
                [JsonProperty] protected readonly int weight = 0;
            }

            [JsonProperty] private readonly List<RankingInfo<T>> rankings = new();

            [JsonIgnore] public T Max
            {
                get
                {
                    if (this.rankings.Count < 1)
                        return default;

                    RankingInfo<T> max = this.rankings[0];
                    foreach (RankingInfo<T> actionRanking in this.rankings)
                    {
                        if (actionRanking.Weight > max.Weight)
                            max = actionRanking;
                    }

                    return max.Value;
                }
            }

            public T Roll()
            {
                List<T> actionPool = new();
                int total = 0;
                foreach (RankingInfo<T> actionRanking in this.rankings)
                {
                    total += actionRanking.Weight;
                    for (int i = 0; i < actionRanking.Weight; i++)
                        actionPool.Add(actionRanking.Value);
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
            IEnumerable<TargetedCharacterStatsAction> targetedGems = CombatManager.Instance.CurrentCharacter.CurrentCombatAction.GetTargetedGems();

            foreach (TargetedCharacterStatsAction targetedGem in targetedGems)
            {
                switch (targetedGem.Target)
                {
                    case CharacterTarget.Self:
                        targetedGem.TargetInfo.CombatTargets = new List<CharacterCombat> { CombatManager.Instance.CurrentCharacter };
                        break;
                    case CharacterTarget.Ally:
                        targetedGem.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAnEnemy(targetedGem.Gem) };
                        break;
                    case CharacterTarget.AllAllies:
                        targetedGem.TargetInfo.CombatTargets = CombatManager.Instance.Players;
                        break;
                    case CharacterTarget.Opponent:
                        targetedGem.TargetInfo.CombatTargets = new List<CharacterCombat> { PickAPlayer(targetedGem.Gem) };
                        break;
                    case CharacterTarget.AllOpponents:
                        targetedGem.TargetInfo.CombatTargets = CombatManager.Instance.Enemies;
                        break;
                    case CharacterTarget.Any:
                        targetedGem.TargetInfo.CombatTargets = new List<CharacterCombat> { PickACharacter(targetedGem.Gem) };
                        break;
                    case CharacterTarget.AllOfAny:
                        //targetedGem.TargetInfo.CombatTargets = ;
                        break;
                    case CharacterTarget.All:
                        targetedGem.TargetInfo.CombatTargets = CombatManager.Instance.Characters;
                        break;
                    default:
                        Terminal.IO.LogError("[CharacterCombatAI:SetupTargetedGems] The provided value for a character target was invalid");
                        break;
                }
            }

            CombatManager.Instance.RunCombatEvents();
        }

        private void RollItemAction()
        {
            ItemInfo item = RollForUtility(CombatManager.Instance.CurrentCharacter.Stats.Items) as ItemInfo;
            CombatManager.Instance.SetupItemCombat(item);
        }

        private CombatActionType RollActionType()
        {
            return this.actionRanking.Roll();
        }

        private IEntityInfo RollForUtility(IEnumerable<IUtilityEntityInfo> entities)
        {
            AuraType aura = this.auraRanking.Roll();
            CombatAlignment alignment = this.alignmentRanking.Roll();

            List<IEntityInfo> list = new();

            foreach (IUtilityEntityInfo entity in entities)
                if (entity.GetUtility.Alignment == alignment)
                    list.Add(entity);

            foreach(IEntityInfo entity in entities)
                if (entity.GetEntity.ContainsAura(aura))
                    list.Add(entity);

            if (list.Count < 1)
                foreach (IUtilityEntityInfo entity in entities)
                    if (entity.GetUtility.Alignment == this.alignmentRanking.Max)
                        list.Add(entity);

            if (list.Count < 1)
                foreach (IEntityInfo entity in entities)
                    if (entity.GetEntity.ContainsAura(this.auraRanking.Max))
                        list.Add(entity);

            if (list.Count < 1)
                foreach (IEntityInfo entity in entities)
                    list.Add(entity);

            int random = UnityEngine.Random.Range(0, list.Count);
            return list[random];
        }

        private CharacterCombat PickAnEnemy(CharacterStatsAction gem)
        { 
            List<CharacterCombat> enemies = new();
            foreach (CharacterCombat enemy in CombatManager.Instance.Enemies)
                enemies.Add(enemy);

            if (gem.IsHealthRestore)
            {
                enemies.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(enemies);
            }

            return enemies[UnityEngine.Random.Range(0, enemies.Count)];
        }

        private CharacterCombat PickAPlayer(CharacterStatsAction gem)
        {
            List<CharacterCombat> players = new();
            foreach (CharacterCombat player in CombatManager.Instance.Players)
                players.Add(player);

            return players[UnityEngine.Random.Range(0, players.Count)];
        }

        private CharacterCombat PickACharacter(CharacterStatsAction gem)
        {
            List<CharacterCombat> characters = new();
            foreach (CharacterCombat character in CombatManager.Instance.Enemies)
                characters.Add(character);

            if (gem.IsHealthRestore && !CombatManager.Instance.CurrentCharacter.Stats.ContainsAura(AuraType.Chaos))
            {
                characters.Sort((x, y) => x.Stats.Health.CompareTo(y.Stats.Health));
                return PickACharacter(characters);
            }

            foreach (CharacterCombat character in CombatManager.Instance.Players)
                characters.Add(character);

            if (gem.Alignment == CombatAlignment.Defense && !CombatManager.Instance.CurrentCharacter.Stats.ContainsAura(AuraType.Chaos))
                return PickAnEnemy(gem);

            if (gem.Alignment == CombatAlignment.Offense && !CombatManager.Instance.CurrentCharacter.Stats.ContainsAura(AuraType.Chaos))
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
    }
}