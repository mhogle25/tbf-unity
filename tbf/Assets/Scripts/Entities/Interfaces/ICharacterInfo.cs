namespace BF2D.Game
{
    public interface ICharacterInfo
    {
        public CharacterStats Stats { get; }
        public int Position { get; set; }
    }
}