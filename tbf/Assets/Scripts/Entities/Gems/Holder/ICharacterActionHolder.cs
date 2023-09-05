namespace BF2D.Game.Actions
{
    public interface ICharacterActionHolder : IUtilityEntityHolder<CharacterActionInfo>
    {
        /// <summary>
        /// Removes a single gem from the holder. This method will delete the gem's datafile if its count reaches zero and if it is a generated entity. Use when customizing (transforming) or deleting an entity.
        /// </summary>
        /// <param name="info">The gem info to remove</param>
        public void Destroy(CharacterActionInfo info);

        /// <summary>
        /// Removes a single gem from the holder. This method will delete the gem's datafile if its count reaches zero and if it is a generated entity. Use when customizing (transforming) or deleting an entity.
        /// </summary>
        /// <param name="id">The gem id to remove</param>
        public void Destroy(string id);
    }
}