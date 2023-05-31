using System;

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

            Actions.CharacterStatsAction gem = gameInfo.GetGem(gemID);

            ItemCustomizer itemCustomizer = new(itemInfo, character.Items);

            Utilities.FileWriter? writer = null;
            try
            {
                itemCustomizer.SetIndex(int.Parse(gemIndex));
                writer = itemCustomizer.EmbueGem(gem.ID, newName);
            }
            catch (Exception x)
            {
                Terminal.IO.LogError(x.Message);
                return;
            }

            if ((bool) writer?.FileExistsStreaming)
            {
                Terminal.IO.LogError($"Can't embue {itemInfo.Name} with new ID '{writer?.ID}'. A static item with that ID already exists.");
                return;
            }

            if ((bool) writer?.FileExists)
            {
                Terminal.IO.LogError($"Can't embue {itemInfo.Name} with new ID '{writer?.ID}'. A custom item with that ID already exists.");
                return;
            }

            writer?.Overwrite();
            Terminal.IO.Log($"Embued {itemInfo.Name} with {gem.Name} as {newName}.");
            character.Items.AcquireItem(writer?.ID);
            Terminal.IO.Log($"Gave {newName} to {character.Name}.");
            gameInfo.SaveGame();
        }
    }
}