using System.Collections.Generic;

namespace BF2D.Game.Actions
{
    public interface ICharacterStatsActionHolder : IEnumerable<CharacterStatsActionInfo>
    {
        public CharacterStatsActionInfo AcquireGem(string id);

        public string ExtractGem(CharacterStatsActionInfo info);

        public CharacterStatsActionInfo TransferGem(CharacterStatsActionInfo info, ICharacterStatsActionHolder receiver);

        public CharacterStatsActionInfo GetGem(string id);
    }
}