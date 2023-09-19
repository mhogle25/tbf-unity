using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public interface ICharacterController
    {
        public CharacterStats Stats { get; }

        public void RunTargetedGameAction(TargetedGameAction gameAction, Action callback);
        public void RunUntargetedGameAction(UntargetedGameAction gameAction, Action callback);
    }
}
