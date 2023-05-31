using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public class TcEmbueItem
    {
        const string useage = "Useage: embueitem [itemID] [characterID] [gemID] [gemIndex] [newName]";

        public static void Run(string[] arguments)
        {
            if (arguments.Length < 6)
            {
                Terminal.IO.LogWarning(TcEmbueItem.useage);
                return;
            }

            if (arguments.Length > 6)
            {
                Terminal.IO.LogWarning(TcEmbueItem.useage);
                return;
            }

            string itemID = arguments[1];
            string characterID = arguments[2];
            string gemID = arguments[3];
            string gemIndex = arguments[4];
            string newName = arguments[5];

            GameInfo gameInfo = GameInfo.Instance;

            if (!gameInfo.SaveActive)
            {
                Terminal.IO.LogError($"Tried to give {newName} to a character but no party was active. Try loading a save file first.");
                return;
            }

            CharacterStats character = gameInfo.GetActivePlayer(characterID);

            ItemInfo itemInfo = character.Items.GetItem(itemID);
            if (itemInfo is null)
            {
                Terminal.IO.LogError($"Tried to embue an item that wasn't in {character.Name}'s inventory.");
                return;
            }

            CharacterStatsActionInfo gemInfo = gameInfo.PartyGems.GetGem(gemID);
            if (gemInfo is null)
            {
                Terminal.IO.LogError($"Tried to embue an item with a gem that wasn't in the party's inventory.");
                return;
            }

            ItemCustomizer itemCustomizer = new(itemInfo, character.Items);

            try
            {
                itemCustomizer.SetIndex(int.Parse(gemIndex));
            }
            catch (Exception x)
            {
                Terminal.IO.LogError(x.Message);
                return;
            }

            Utilities.FileWriter writer = itemCustomizer.EmbueGem(gemInfo, gameInfo.PartyGems, newName);
            if (writer is null)
                return;

            if ((bool) writer.FileExistsStreaming)
            {
                Terminal.IO.LogError($"Can't embue {itemInfo.Name} with new ID '{writer.ID}'. A static item with that ID already exists.");
                return;
            }

            if ((bool) writer.FileExists)
            {
                Terminal.IO.LogError($"Can't embue {itemInfo.Name} with new ID '{writer.ID}'. A custom item with that ID already exists.");
                return;
            }

            writer.Overwrite();
            Terminal.IO.Log($"Embued {itemInfo.Name} with {gemInfo.Name} as {newName}.");
            Terminal.IO.Log($"Gave {newName} to {character.Name}.");
            gameInfo.SaveGame();
        }
    }
}