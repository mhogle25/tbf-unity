using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IUtilityEntityHolder<T> : IEnumerable<T>
    {
        public T Acquire(string id);

        public T Transfer(T info, IUtilityEntityHolder<T> receiver);

        public T Transfer(string id, IUtilityEntityHolder<T> receiver);

        /// <summary>
        /// Extracts an entity from the holder and returns its ID
        /// </summary>
        /// <param name="info">The entity to extract</param>
        /// <returns>The id of the entity</returns>
        public string Extract(T info);

        /// <summary>
        /// Extracts an entity from the holder and returns its ID
        /// </summary>
        /// <param name="id">The entity id to extract</param>
        /// <returns>The id of the entity</returns>
        public string Extract(string id);

        public T Get(string id);
    }
}