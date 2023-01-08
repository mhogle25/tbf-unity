using System.Collections.Generic;

namespace BF2D.Combat.Actions
{
    public interface ICombatActionInfo
    {
        public List<string> GetOpeningMessage();
    }
}
