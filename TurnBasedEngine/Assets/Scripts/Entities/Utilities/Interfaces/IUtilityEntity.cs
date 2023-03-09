namespace BF2D.Game
{
    public interface IUtilityEntity
    {
        public string SpriteID { get; }

        public Enums.CombatAlignment Alignment { get; }
    }
}