namespace BF2D.Game
{
    public interface IUtilityEntityInfo : IEntityInfo
    {
        public IUtilityEntity GetUtility { get; }
    }
}