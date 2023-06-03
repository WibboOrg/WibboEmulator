namespace WibboEmulator.Communication.Packets.Incoming.Inventory.Furni;

using WibboEmulator.Database.Daos.Item;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class UpgradeFurniRareInventoryEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (session.User.InventoryComponent == null)
        {
            return;
        }

        var itemId = packet.PopInt();

        var item = session.User.InventoryComponent.GetItem(itemId);

        if (item == null)
        {
            return;
        }

        if (item.GetBaseItem().RarityLevel is RaretyLevelType.None or RaretyLevelType.Legendary)
        {
            return;
        }

        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(1635463734, out var pageLegendary);
        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(1635463733, out var pageEpic);
        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(1635463732, out var pageCommun);
        _ = WibboEnvironment.GetGame().GetCatalog().TryGetPage(1635463731, out var pageBasic);

        if (pageLegendary == null || pageEpic == null || pageCommun == null || pageBasic == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        if (item.GetBaseItem().RarityLevel == RaretyLevelType.Basic && pageBasic.ItemOffers.TryGetValue(item.BaseItem, out var basicOffers))
        {
            return;
        }

        ItemData lotData;
        int amount;
        if (item.GetBaseItem().RarityLevel == RaretyLevelType.Epic)
        {
            lotData = pageLegendary.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, pageLegendary.Items.Count - 1)).Value.Data;
            amount = 10;
        }
        else if (item.GetBaseItem().RarityLevel == RaretyLevelType.Commun)
        {
            lotData = pageEpic.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, pageEpic.Items.Count - 1)).Value.Data;
            amount = 10;
        }
        else
        {
            lotData = pageCommun.Items.ElementAt(WibboEnvironment.GetRandomNumber(0, pageCommun.Items.Count - 1)).Value.Data;
            amount = 50;
        }

        if (lotData == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.error", session.Langue));
            return;
        }

        var items = session.User.InventoryComponent.GetItemsByType(item.BaseItem, amount);

        if (items.Count < amount)
        {
            return;
        }

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

        session.User.InventoryComponent.DeleteItems(dbClient, items);

        var newItem = ItemFactory.CreateSingleItemNullable(dbClient, lotData, session.User, "", 0, 0);

        session.User.InventoryComponent.TryAddItem(newItem);

        if (lotData.Amount >= 0)
        {
            lotData.Amount += 1;
            ItemStatDao.UpdateAdd(dbClient, lotData.Id, 1);
        }
    }
}
