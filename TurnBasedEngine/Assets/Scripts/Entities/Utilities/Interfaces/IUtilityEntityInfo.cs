using UnityEngine;

namespace BF2D.Game
{
    public interface IUtilityEntityInfo : IEntityInfo
    {
        public IUtilityEntity GetUtility();

        public Sprite Icon { get; }

        public int Increment();

        public int Decrement();
    }
}