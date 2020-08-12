public class Stats
{
    public int Health { get { return _health; } }
    private int _health;
    public int MaxHealth { get { return _maxHealth; } }
    private int _maxHealth;
    public int Stamina { get { return _stamina; } }
    private int _stamina;
    public int MaxStamina { get { return _maxStamina; } }
    private int _maxStamina;
    public int Attack { get { return _attack + _strength; } }
    private int _attack;
    public int Strength { get { return _strength; } }
    private int _strength;
    public int Defense { get { return _defense + _toughness; } }
    private int _defense;
    public int Toughness { get { return _toughness; } }
    private int _toughness;
    public int Focus { get { return _focus + _will; } }
    private int _focus;
    public int Will { get { return _will; } }
    private int _will;
    public int Luck { get { return _luck; } }
    private int _luck;

    public void Damage(int damage) {
        _health -= (damage - Defense) > 0 ? (damage - Defense) : 1;
    }
}
