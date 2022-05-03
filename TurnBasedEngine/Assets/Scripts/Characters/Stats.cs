namespace BF2D.Combat
{
    public abstract class Stats
    {
        public string Name { get { return this.name; } set { this.name = value; } }
        private string name = string.Empty;
        public string Description { get { return this.description; } set { this.description = value; } }
        private string description = string.Empty;
        public int Health { get { return this.health; } }
        private protected int health = 0;
        public int MaxHealth { get { return this.maxHealth; } }
        private protected int maxHealth = 0;
        public int Stamina { get { return this.stamina; } }
        private protected int stamina = 0;
        public int MaxStamina { get { return this.maxStamina; } }
        private protected int maxStamina = 0;
        public int Speed { get { return this.speed + this.swiftness; } }
        private protected int speed = 0;
        public int Swiftness { get { return this.swiftness; } }
        private protected int swiftness = 0;
        public int Attack { get { return this.attack + this.strength; } }
        private protected int attack = 0;
        public int Strength { get { return this.strength; } }
        private protected int strength = 0;
        public int Defense { get { return this.defense + this.toughness; } }
        private protected int defense = 0;
        public int Toughness { get { return this.toughness; } }
        private protected int toughness = 0;
        public int Focus { get { return this.focus + this.will; } }
        private protected int focus = 0;
        public int Will { get { return this.will; } }
        private protected int will = 0;
        public int Luck { get { return this.luck; } }
        private protected int luck = 0;

        public void Damage(int damage)
        {
            this.health -= (damage - this.Defense) > 0 ? (damage - this.Defense) : 1;
        }

        public void CriticalDamage(int damage)
        {
            this.health -= damage > 0 ? damage : 1;
        }

        public void PsychicDamage(int damage)
        {
            this.health -= (damage - this.Focus) > 0 ? (damage - this.Focus) : 1;
        }

        public void Heal(int healing)
        {
            this.health += healing > 0 ? healing : 1;
        }

        public void Recover(int recovery)
        {
            this.stamina += recovery > 0 ? recovery : 1;
        }

        public void Exert(int exertion)
        {
            this.stamina -= exertion > 0 ? exertion : 1;
        }

        public void ResetHealth()
        {
            this.health = this.maxHealth;
        }

        public void ResetStamina()
        {
            this.stamina = this.maxStamina;
        }

        public void ResetStats()
        {
            ResetHealth();
            ResetStamina();
        }
    }
}
