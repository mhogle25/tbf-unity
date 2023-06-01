using System.Collections.Generic;

namespace BF2D.Game.Actions
{
    public interface ICharacterStatsActionHolder : IUtilityEntityHolder<CharacterStatsActionInfo>
    {
        public string Extract(CharacterStatsActionInfo info);
    }
}