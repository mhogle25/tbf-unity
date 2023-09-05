using System.Collections.Generic;

namespace BF2D.Game
{
    public interface IUtilityEntityHolder<T> : IEnumerable<T> where T : UtilityEntityInfo
    {
        public T Acquire(string id);

        public T Transfer(T info, IUtilityEntityHolder<T> holder);

        public T Transfer(string id, IUtilityEntityHolder<T> holder);

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