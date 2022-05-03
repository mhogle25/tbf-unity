using System.Collections.Generic;

namespace BF2D.Combat
{
    public class PlayerStats : Stats
    {
        public Job Profession { get { return this.profession; } }
        private Job profession = null;
        public int Experience { get { return this.experience; } }
        private int experience = 0;
        public int Level { get { return this.level; } }
        private int level = 0;
        public List<Item> Inventory { get { return this.inventory; } }
        private List<Item> inventory = new List<Item>();

    }
}
