using UnityEngine;
using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public static class TcAcquireGems
    {
        const string useage = "Useage: acquiregems [gemID] (optional ->) [count]";

        public static void Run(string[] arguments)
        {
            if (arguments.Length > 3)
            {
                Terminal.IO.LogWarning(useage);
                return;
            }

            if (arguments.Length < 2)
            {
                Terminal.IO.LogWarning(useage);
                return;
            }

            if (!GameCtx.Instance.SaveActive)
            {
                Terminal.IO.LogError("Tried to acquire gems for a party but there was no save file loaded.");
                return;
            }

            string gemID = arguments[1];
            string countString = string.Empty;
            if (arguments.Length == 3)
                countString = arguments[2];

            int count = 1;
            try
            {
                if (!string.IsNullOrEmpty(countString))
                    count = int.Parse(countString);
            }
            catch (Exception x)
            {
                Terminal.IO.LogError(x.Message);
                return;
            }

            int i = 0;
            CharacterStatsActionInfo gemInfo = null;
            for (; i < count; i++)
                gemInfo = GameCtx.Instance.PartyGems.Acquire(gemID);

            Terminal.IO.Log($"The party acquired {i} {gemInfo.Name}s.");
        }
    }
}