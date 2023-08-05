using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    class SCListPlayers
    {
        private const string useage = "Useage: players [active OR inactive]";

        public static void Run(params string[] arguments)
        {
            if (arguments.Length > 2 || arguments.Length < 2)
            {
                ShCtx.One.LogWarning(SCListPlayers.useage);
                return;
            }

            string status = arguments[1];

            GameCtx ctx = GameCtx.One;

            if (!ctx.SaveActive)
            {
                ShCtx.One.LogWarning("Tried to list players but no save file was loaded");
                return;
            }

            switch (status)
            {
                case "active":
                    ListPlayers(ctx.ActivePlayers);
                    break;
                case "inactive":
                    ListPlayers(ctx.InactivePlayers);
                    break;
                default:
                    ShCtx.One.LogWarning(SCListPlayers.useage);
                    return;
            }
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
