using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Game.Combat
{
    public class SCCombat
    {
        public static void Run(params string[] arguments)
        {
            const string warning = "Useage: combat [saveFileID] [encounterFactoryID]";

            if (arguments.Length < 3 || arguments.Length > 3)
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

            string encounterFactoryID = arguments[2];
            Encounter encounter = GameCtx.One.GetEncounterFactory(encounterFactoryID).InstantiateEncounter();

            CombatCtx.One.CancelCombat();

            GameCtx.One.StageEncounter(encounter); 
        }
    }
}