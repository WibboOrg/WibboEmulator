using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ExtraBox : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            int.TryParse(Params[1], out int NbLot);

            if (NbLot < 0 || NbLot > 10)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(73917766, out ItemData ItemData))
            {
                return;
            }

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetUser(), "", NbLot);
            foreach (Item PurchasedItem in Items)
            {
                if (Session.GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    Session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }
        }
    }
}
