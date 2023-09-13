using UnityEngine;
using BF2D.Game.Enums;
using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IUtilityEntity : ICombatAligned
    {
        public string ID { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<AuraType> Auras { get; }

        public bool Chaotic { get; }

        public bool IsRestoration { get; }

        public bool ContainsAura(AuraType aura);

        public void EmbueAura(AuraType aura);

        public void RemoveAura(AuraType aura);

        public string SpriteID { get; }

        public Sprite GetIcon();
    }
}