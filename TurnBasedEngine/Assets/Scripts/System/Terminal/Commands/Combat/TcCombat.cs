using System.Collections.Generic;

namespace BF2D.Game.Combat
{
    public class TcCombat
    {
        public static void Run(string[] arguments)
        {
            const string warning = "Useage: combat [saveFileID] [enemyID1] (optional ->) [enemyID2]...";

            if (arguments.Length < 3)
            {
                Terminal.IO.LogWarning(warning);
                return;
            }

            string saveFileID = arguments[1];

            GameCtx.Instance.LoadGame(saveFileID);
            if (!GameCtx.Instance.SaveActive)
            {
                Terminal.IO.LogError("Save file load failed.");
                return;
            }

            List<CharacterStats> enemies = new();
            for (int i = 2; i < arguments.Length; i++)
            {
                CharacterStats enemy = GameCtx.Instance.InstantiateEnemy(arguments[i]);
                if (enemy is null)
                {
                    Terminal.IO.LogError($"Enemy with ID {arguments[i]} does not exist.");
                    return;
                }
                enemies.Add(enemy);
            }

            CombatManager.Instance.CancelCombat();

            GameCtx.Instance.StageCombatInfo(new CombatManager.InitializeInfo
            {
                players = GameCtx.Instance.ActivePlayers,
                enemies = enemies
            });
        }
    }
}