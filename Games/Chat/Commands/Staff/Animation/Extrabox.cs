namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class ExtraBox : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {

        _ = int.TryParse(parameters[1], out var NbLot);

        if (NbLot is < 0 or > 10)
        {
            return;
        }

        var lootboxId = WibboEnvironment.GetSettings().GetData<int>("givelot.lootbox.id");
        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(lootboxId, out var ItemData))
        {
            return;
        }

        var Items = ItemFactory.CreateMultipleItems(ItemData, session.GetUser(), "", NbLot);
        foreach (var PurchasedItem in Items)
        {
            if (session.GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
            {
                session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
            }
        }
    }
}
