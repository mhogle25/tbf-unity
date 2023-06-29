using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game
{
    class TcListPlayers
    {
        private const string useage = "Useage: players [active OR inactive]";

        public static void Run(string[] arguments)
        {
            if (arguments.Length > 2 || arguments.Length < 2)
            {
                Terminal.IO.LogWarning(TcListPlayers.useage);
                return;
            }

            string status = arguments[1];

            GameCtx ctx = GameCtx.Instance;

            if (!ctx.SaveActive)
            {
                Terminal.IO.LogWarning("Tried to list players but no save file was loaded");
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
                    Terminal.IO.LogWarning(TcListPlayers.useage);
                    return;
            }
        }

        private static void ListPlayers(CharacterStats[] collection)
        {
            for (int i = 0; i < collection.Length; i++)
            {
                CharacterStats character = collection[i];
                string deadOrAlive = character.Dead ? "D" : "A";
                Terminal.IO.Log($"{i} {deadOrAlive} {character.Name}");
            }
        }
    }
}
