namespace BF2D.Game
{
    public interface IExerter
    {
        protected int ExertionCost(CharacterStats source);

        protected string ExertionCostText(CharacterStats source);

        protected bool HasExertionCost { get; }
    }
}