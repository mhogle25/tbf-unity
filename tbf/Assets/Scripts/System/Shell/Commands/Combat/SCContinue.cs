using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game.Combat
{
    public static class SCContinue
    {
        public static void Run(params string[] arguments)
        {
            CombatCtx.One.CurrentCharacter.DEBUG_CONTINUE();
        }
    }
}