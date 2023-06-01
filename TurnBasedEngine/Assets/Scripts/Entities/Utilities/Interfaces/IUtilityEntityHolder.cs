using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IUtilityEntityHolder<T> : IEnumerable<T>
    {
        public T Acquire(string id);

        public T Transfer(T info, IUtilityEntityHolder<T> receiver);

        public T Get(string id);
    }
}