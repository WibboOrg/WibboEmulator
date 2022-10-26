namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class ExtraBox : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {

        _ = int.TryParse(parameters[1], out var nbLot);

        if (nbLot is < 0 or > 10)
        {
            return;
        }

        var lootboxId = WibboEnvironment.GetSettings().GetData<int>("givelot.lootbox.id");
        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(lootboxId, out var itemData))
        {
            return;
        }

        var items = ItemFactory.CreateMultipleItems(itemData, session.User, "", nbLot);
        foreach (var purchasedItem in items)
        {
            if (session.User.InventoryComponent.TryAddItem(purchasedItem))
            {
                session.SendPacket(new FurniListNotificationComposer(purchasedItem.Id, 1));
            }
        }
    }
}
