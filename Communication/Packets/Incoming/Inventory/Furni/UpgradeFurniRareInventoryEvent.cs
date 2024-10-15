namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;

using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Utilities;

internal sealed class UpgradeFurniRareInventoryEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (Session.User.InventoryComponent == null)
        {
            return;
        }

        var itemId = packet.PopInt();

        var item = Session.User.InventoryComponent.GetItem(itemId);

        if (item == null)
        {
            return;
        }

        if (item.ItemData.RarityLevel is RaretyLevelType.None or RaretyLevelType.Legendary)
        {
            return;
        }

        _ = CatalogManager.TryGetPage(1635463734, out var pageLegendary);
        _ = CatalogManager.TryGetPage(1635463733, out var pageEpic);
        _ = CatalogManager.TryGetPage(1635463732, out var pageCommun);
        _ = CatalogManager.TryGetPage(1635463731, out var pageBasic);

        if (pageLegendary == null || pageEpic == null || pageCommun == null || pageBasic == null)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.error", Session.Language));
            return;
        }

        if (item.ItemData.RarityLevel == RaretyLevelType.Basic && pageBasic.ItemOffers.TryGetValue(item.BaseItemId, out var basicOffers))
        {
            return;
        }

        ItemData lotData;
        int amount;
        if (item.ItemData.RarityLevel == RaretyLevelType.Epic)
        {
            lotData = pageLegendary.Items.GetRandomElement().Value.Data;
            amount = 10;
        }
        else if (item.ItemData.RarityLevel == RaretyLevelType.Commun)
        {
            lotData = pageEpic.Items.GetRandomElement().Value.Data;
            amount = 10;
        }
        else
        {
            lotData = pageCommun.Items.GetRandomElement().Value.Data;
            amount = 50;
        }

        if (lotData == null)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.error", Session.Language));
            return;
        }

        var items = Session.User.InventoryComponent.GetItemsByType(item.BaseItemId, amount);

        if (items.Count < amount)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        Session.User.InventoryComponent.DeleteItems(dbClient, items);

        var newItem = ItemFactory.CreateSingleItemNullable(dbClient, lotData, Session.User, "", 0, 0);

        Session.User.InventoryComponent.TryAddItem(newItem);

        if (lotData.Amount >= 0)
        {
            lotData.Amount += 1;
            ItemStatDao.UpdateAdd(dbClient, lotData.Id, 1);
        }
    }
}
