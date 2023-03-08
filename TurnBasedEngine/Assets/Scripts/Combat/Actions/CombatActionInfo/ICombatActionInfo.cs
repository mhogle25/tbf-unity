using System.Collections.Generic;

namespace BF2D.Game.Combat.Actions
{
    public interface ICombatActionInfo
    {
        public List<string> GetOpeningMessage();
    }
}
