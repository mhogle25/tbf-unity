using UnityEngine;
using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public static class SCAcquireGems
    {
        const string useage = "Useage: acquiregems [gemID] (optional ->) [count]";

        public static void Run(params string[] arguments)
        {
            if (arguments.Length > 3)
            {
                ShCtx.One.LogWarning(useage);
                return;
            }

            if (arguments.Length < 2)
            {
                ShCtx.One.LogWarning(useage);
                return;
            }

            if (!GameCtx.One.SaveActive)
            {
                ShCtx.One.LogError("Tried to acquire gems for a party but there was no save file loaded.");
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
                ShCtx.One.LogError(x.Message);
                return;
            }

            int i = 0;
            CharacterActionInfo gemInfo = null;
            for (; i < count; i++)
                gemInfo = GameCtx.One.Gems.Acquire(gemID);

            ShCtx.One.Log($"The party acquired {i} {gemInfo.Name}s.");
        }
    }
}