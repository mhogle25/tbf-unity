using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    class SCListPlayers
    {
        private const string useage = "Useage: players";

        public static void Run(params string[] arguments)
        {
            if (arguments.Length > 1 || arguments.Length < 1)
            {
                ShCtx.One.LogWarning(SCListPlayers.useage);
                return;
            }

            GameCtx ctx = GameCtx.One;

            if (!ctx.SaveActive)
            {
                ShCtx.One.LogWarning("Tried to list players but no save file was loaded");
                return;
            }

            ListPlayers(ctx.ActivePlayers);
        }

        private static void ListPlayers(CharacterStats[] collection)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                CharacterStats character = collection[i];
                string deadOrAlive = character.Dead ? "D" : "A";
                ShCtx.One.Log($"{i} {deadOrAlive} {character.Name}");
            }
        }
    }
}
