using System.Collections.Generic;
using System.Linq;

namespace BF2D.Game
{
    public abstract class CharacterGroup
    {
        public abstract int ActiveCharacterCount { get; }
        public abstract IEnumerable<ICharacterInfo> ActiveCharacters { get; }
        public abstract ICharacterInfo Leader { get; }

        /// <summary>
        /// Changes the group leader
        /// </summary>
        /// <param name="newLeader">The new leader</param>
        /// <returns>The old leader</returns>
        public abstract ICharacterInfo ChangeLeader(ICharacterInfo newLeader);

        protected int GetNextAvailablePosition()
        {
            if (this.Leader is null)
                return 0;

            HashSet<int> used = this.ActiveCharacters.Select(character => character.Position).ToHashSet();

            int i = 1;
            for (; i < Numbers.maxPartySize; i++)
            {
                if (!used.Contains(i))
                    break;
            }

            return i;
        }
    }
}