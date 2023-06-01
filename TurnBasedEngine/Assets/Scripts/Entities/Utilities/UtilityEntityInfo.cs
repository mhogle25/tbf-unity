using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    public abstract class UtilityEntityInfo : IEntityInfo
    {
        public abstract string ID { get; set; }

        public abstract int Count { get; }

        public abstract Entity GetEntity();

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract IEnumerable<Enums.AuraType> Auras { get; }

        public abstract IUtilityEntity GetUtility();

        public abstract Sprite Icon { get; }

        public abstract int Increment();

        public abstract int Decrement();
    }
}