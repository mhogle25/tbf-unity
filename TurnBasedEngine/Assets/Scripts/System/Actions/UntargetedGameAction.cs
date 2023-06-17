using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using BF2D.Game.Enums;
using UnityEngine;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class UntargetedGameAction : GameAction, ICache, ICombatAligned
    {
        [JsonProperty] private readonly string[] gemIDs = { };

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
        [JsonIgnore] public int Count => this.gemIDs.Length;

        [JsonIgnore] private readonly List<CharacterStatsAction> cached = new();

        [JsonIgnore] public CombatAlignment Alignment => CombatAlignmentSelector.CalculateCombatAlignedCollection(this.Gems);

        public void Clear()
        {
            this.cached.Clear();
        }

        [JsonConstructor]
        public UntargetedGameAction()
        {
            GameCtx.Instance.RegisterCache(this);
        }

        public UntargetedGameAction(string[] gemIDs)
        {
            GameCtx.Instance.RegisterCache(this);
            this.gemIDs = gemIDs;
        }

        ~UntargetedGameAction()
        {
            GameCtx.Instance.RemoveExternalCache(this);
        }

        public void SetGemIDAt(string gemID, int index)
        {
            if (index < 0 || index >= this.gemIDs.Length)
            {
                Debug.LogError("[UntargetedGameAction:SetGemIDAt] Index was outside the bounds of the array");
                return;
            }

            CharacterStatsAction gem = GameCtx.Instance.GetGem(gemID);

            if (gem is null)
            {
                Debug.LogError($"[UntargetedGameAction:SetGemIDAt] The gem at id '{gemID}' does not exist.");
                return;
            }

            if (gem.HasStatsUp)
            {
                Debug.LogError("[UntargetedGameAction:SetGemIDAt] Cannot embue a stats modifier gem to an untargeted game action.");
                return;
            }

            this.gemIDs[index] = gemID;
        }
    }
}