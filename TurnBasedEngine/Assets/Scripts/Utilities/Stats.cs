
public class Stats
{
    public int Health { get { return _health; } }
    private protected int _health;
    public int MaxHealth { get { return _maxHealth; } }
    private protected int _maxHealth;
    public int Stamina { get { return _stamina; } }
    private protected int _stamina;
    public int MaxStamina { get { return _maxStamina; } }
    private protected int _maxStamina;
    public int Speed { get { return _speed + _swiftness; } }
    private protected int _speed;
    public int Swiftness { get { return _swiftness; } }
    private protected int _swiftness;
    public int Attack { get { return _attack + _strength; } }
    private protected int _attack;
    public int Strength { get { return _strength; } }
    private protected int _strength;
    public int Defense { get { return _defense + _toughness; } }
    private protected int _defense;
    public int Toughness { get { return _toughness; } }
    private protected int _toughness;
    public int Focus { get { return _focus + _will; } }
    private protected int _focus;
    public int Will { get { return _will; } }
    private protected int _will;
    public int Luck { get { return _luck; } }
    private protected int _luck;

    public void Damage(int damage) {
        _health -= (damage - Defense) > 0 ? (damage - Defense) : 1;
    }

    public void CriticalDamage(int damage) {
        _health -= damage > 0 ? damage : 1;
    }

    public void PsychicDamage(int damage) {
        _health -= (damage - Focus) > 0 ? (damage - Focus) : 1;
    }

    public void Heal(int healing) {
        _health += healing > 0 ? healing : 1;
    }

    public void ResetHealth() {
        _health = _maxHealth;
    }

    public void Exert(int exertion) {
        _stamina -= exertion;
    }

    public void Recover(int recovery) {
        _stamina += recovery;
    }

    public void ResetStamina() {
        _stamina = _maxStamina;
    }

    public void ResetStats() {
        ResetHealth();
        ResetStamina();
    }
}
