using System.Collections.Generic;

public class PlayerStats : Stats
{
    public Job Profession { get { return _profession; } }
    private Job _profession;
    public int Experience { get { return _experience; } }
    private int _experience;
    public int Level { get { return _level; } }
    private int _level;
    public List<Item> Inventory { get { return _inventory; } }
    private List<Item> _inventory = new List<Item>();
}
