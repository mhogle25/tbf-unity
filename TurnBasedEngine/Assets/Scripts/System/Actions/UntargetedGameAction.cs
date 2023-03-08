using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
namespace BF2D.Game.Actions
{
    public class UntargetedGameAction : GameAction, ICache
    {
        [JsonIgnore] public IEnumerable<CharacterStatsAction> Gems 
        { 
            get 
            {
                if (this.cached.Count > 0)
                    return this.cached;

                foreach (string id in this.gemIDs)
                {
                    CharacterStatsAction gem = GameInfo.Instance.GetGem(id);
                    if (gem is not null)
                        this.cached.Add(gem);
                }
                return this.cached;
            } 
        }
        [JsonProperty] private readonly List<string> gemIDs = new();
        [JsonIgnore] private readonly List<CharacterStatsAction> cached = new();

        public void Clear()
        {
            this.cached.Clear();
        }

        public UntargetedGameAction()
        {
            GameInfo.Instance.RegisterCache(this);
        }

        ~UntargetedGameAction()
        {
            GameInfo.Instance.RemoveExternalCache(this);
        }
    }
}