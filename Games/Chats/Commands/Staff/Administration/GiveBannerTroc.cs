namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;

using WibboEmulator.Database;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class GiveBannerTroc : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 3)
        {
            return;
        }

        _ = int.TryParse(parameters[1], out var bannerId);
        _ = int.TryParse(parameters[2], out var nbLot);

        nbLot = nbLot < 0 ? 1 : nbLot > 100 ? 100 : nbLot;

        // TODO: Get by SettingsManager
        var lootboxId = 1000009301; // wibbo_troc_banner
        if (!ItemManager.GetItem(lootboxId, out var itemData))
        {
            return;
        }

        if (!BannerManager.TryGetBannerById(bannerId, out var banner))
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        var items = ItemFactory.CreateMultipleItems(dbClient, itemData, Session.User, banner.Id.ToString(), nbLot);
        foreach (var purchasedItem in items)
        {
            Session.User.InventoryComponent.TryAddItem(purchasedItem);
        }
    }
}
