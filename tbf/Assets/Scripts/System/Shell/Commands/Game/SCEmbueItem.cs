using System;
using BF2D.Game.Actions;

namespace BF2D.Game
{
    public class SCEmbueItem
    {
        const string useage = "Useage: embueitem [itemID] [activePlayerIndex] [gemID] [gemIndex] [newName]\nList active players using 'players'";

        public static void Run(params string[] arguments)
        {
            if (arguments.Length < 6)
            {
                ShCtx.One.LogWarning(SCEmbueItem.useage);
                return;
            }

            if (arguments.Length > 6)
            {
                ShCtx.One.LogWarning(SCEmbueItem.useage);
                return;
            }

            string itemID = arguments[1];
            string activePlayerIndex = arguments[2];
            string gemID = arguments[3];
            string gemIndex = arguments[4];
            string newName = arguments[5];

            int playerIndex;
            try
            {
                playerIndex = int.Parse(activePlayerIndex);
            }
            catch
            {
                ShCtx.One.LogWarning(SCEmbueItem.useage);
                return;
            }

            GameCtx ctx = GameCtx.One;

            if (!ctx.SaveActive)
            {
                ShCtx.One.LogError($"Tried to give {newName} to a character but no party was active. Try loading a save file first.");
                return;
            }

            CharacterStats character = ctx.ActivePlayers[playerIndex];

            ItemInfo itemInfo = character.Items.Get(itemID);
            if (itemInfo is null)
            {
                ShCtx.One.LogError($"Tried to embue an item that wasn't in {character.Name}'s inventory.");
                return;
            }

            CharacterActionInfo gemInfo = ctx.Gems.Get(gemID);
            if (gemInfo is null)
            {
                ShCtx.One.LogError($"Tried to embue an item with a gem that wasn't in the party's inventory.");
                return;
            }

            ItemCustomizer itemCustomizer = new(itemInfo, character.Items);

            try
            {
                itemCustomizer.SetIndex(int.Parse(gemIndex));
            }
            catch (Exception x)
            {
                ShCtx.One.LogError(x.Message);
                return;
            }

            Utilities.FileWriter writer = itemCustomizer.EmbueGem(gemInfo, ctx.Gems, newName);
            if (writer is null)
                return;

            if (writer.FileExistsStreaming)
            {
                ShCtx.One.LogError($"Can't embue {itemInfo.Name} with new ID '{writer.ID}'. A static item with that ID already exists.");
                return;
            }

            if (writer.FileExists)
            {
                ShCtx.One.LogError($"Can't embue {itemInfo.Name} with new ID '{writer.ID}'. A custom item with that ID already exists.");
                return;
            }

            writer.Overwrite();
            ShCtx.One.Log($"Embued {itemInfo.Name} with {gemInfo.Name} as {newName}.");
            ShCtx.One.Log($"Gave {newName} to {character.Name}.");
            ctx.SaveGame();
        }
    }
}