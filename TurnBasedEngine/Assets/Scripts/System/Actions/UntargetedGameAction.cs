using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class UntargetedGameAction : GameAction, ICache, ICombatAligned
    {
        [JsonIgnore] public IEnumerable<CharacterStatsAction> Gems 
        { 
            get 
            {
                if (this.cached.Count > 0)
                    return this.cached;

                foreach (string id in this.gemIDs)
                {
                    CharacterStatsAction gem = GameCtx.Instance.GetGem(id);
                    if (gem is not null)
                        this.cached.Add(gem);
                }
                return this.cached;
            } 
        }
        [JsonProperty] private readonly string[] gemIDs = { };
        [JsonIgnore] private readonly List<CharacterStatsAction> cached = new();

        [JsonIgnore] public CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.Gems);

        public void Clear()
        {
            this.cached.Clear();
        }

        public UntargetedGameAction()
        {
            GameCtx.Instance.RegisterCache(this);
        }

        ~UntargetedGameAction()
        {
            GameCtx.Instance.RemoveExternalCache(this);
        }
    }
}