using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;

namespace BF2D.Game.Actions
{
    [Serializable]
    public class GameAction
    {
        public class GameActionInfo
        {
            public List<string> postActionDialog;
        }

        [JsonIgnore] public List<CharacterStatsAction> StatsActions { get { return this.statsActions; } }
        [JsonProperty] private List<CharacterStatsAction> statsActions = null;

        public GameActionInfo Run(CharacterStats source, List<CharacterStats> targets)
        {
            string message = string.Empty;
            foreach(CharacterStats target in targets)
            {
                message += RunStatsActions(source, target);
            }

            return new GameActionInfo
            {
                postActionDialog = CreateDialogFromMessage(message)
            };
        }

        private string RunStatsActions(CharacterStats source, CharacterStats target)
        {
            string message = string.Empty;
            foreach (CharacterStatsAction statsAction in this.statsActions)
            {
                message += statsAction.Run(source, target);
            }
            return message;
        }

        private List<string> CreateDialogFromMessage(string text)
        {
            string line;
            StringReader stream = new(text);
            List<string> dialog = new();
            string message = string.Empty;
            int i = 0;
            while (true)
            {
                line = stream.ReadLine();
                if (line is null)
                    break;
                if (i > 5)
                {
                    i = 0;
                    dialog.Add(message);
                    message = string.Empty;
                }
                message += line;
                i++;
            }

            if (message != string.Empty)
                dialog.Add(message);

            return dialog;
        }
    }
}
