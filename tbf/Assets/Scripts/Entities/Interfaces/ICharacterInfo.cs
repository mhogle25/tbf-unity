using BF2D.Game.Combat;

namespace BF2D.Game
{
    public interface ICharacterInfo
    {
        public CharacterStats Stats { get; }
        public int Position { get; set; }
        public ICharacterController CurrentController { get; set; }
    }
}