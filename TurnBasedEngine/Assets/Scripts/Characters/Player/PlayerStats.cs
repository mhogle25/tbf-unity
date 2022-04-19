using System.Collections.Generic;

public class PlayerStats : Stats
{
    public Job Profession { get { return _profession; } }
    private Job _profession = null;
    public int Experience { get { return _experience; } }
    private int _experience = 0;
    public int Level { get { return _level; } }
    private int _level = 0;
    public List<Item> Inventory { get { return _inventory; } }
    private List<Item> _inventory = new List<Item>();

}
