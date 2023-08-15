using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game.Combat
{
    public class SCCombat
    {
        public static void Run(params string[] arguments)
        {
            const string warning = "Useage: combat [saveFileID] [enemyID1] (optional ->) [enemyID2]...";

            if (arguments.Length < 3)
            {
                ShCtx.One.LogWarning(warning);
                return;
            }

            string saveFileID = arguments[1];

            GameCtx.One.LoadGame(saveFileID);
            if (!GameCtx.One.SaveActive)
            {
                ShCtx.One.LogError("Save file load failed.");
                return;
            }

            List<CharacterStats> enemies = new();
            for (int i = 2; i < arguments.Length; i++)
            {
                CharacterStats enemy = GameCtx.One.InstantiateEnemy(arguments[i]);
                if (enemy is null)
                {
                    ShCtx.One.LogError($"Enemy with ID {arguments[i]} does not exist.");
                    return;
                }
                enemies.Add(enemy);
            }

            CombatCtx.One.CancelCombat();

            GameCtx.One.StageCombatInfo(new CombatCtx.InitializeInfo
            {
                players = GameCtx.One.ActivePlayers,
                enemies = enemies
            }); 
        }
    }
}