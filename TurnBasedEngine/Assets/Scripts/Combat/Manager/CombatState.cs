using System;
using BF2D.Game.Actions;

namespace BF2D.Combat
{
    public abstract class CombatState
    {
        public Action Finalize { set { this.finalize = value; } }
        protected Action finalize;

        public abstract void Begin();

        public virtual void ExecuteGameAction(GameAction gameAction)
        {

        }
    }
}