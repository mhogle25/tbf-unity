using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IEntityInfo
    {
        public string ID { get; }

        public string Name { get; }

        public string Description { get; }

        public IEnumerable<Enums.AuraType> Auras { get; }

        public bool ContainsAura(Enums.AuraType aura);
    }
}