namespace BF2D.Game.Actions
{
    public interface ICharacterStatsActionHolder
    {
        public CharacterStatsActionInfo AcquireGem(string id);

        public CharacterStatsActionInfo RemoveGem(CharacterStatsActionInfo info);

        public CharacterStatsActionInfo GetGem(string id);
    }
}