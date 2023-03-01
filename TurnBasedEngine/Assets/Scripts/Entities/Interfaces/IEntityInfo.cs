namespace BF2D.Game
{
    public interface IEntityInfo
    {
        public string ID { get; }

        public Entity GetEntity { get; }
    }
}