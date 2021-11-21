using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class extrabox : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            int.TryParse(Params[1], out int NbLot);

            if (NbLot < 0 || NbLot > 10)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(12018410, out ItemData ItemData))
            {
                return;
            }

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, Session.GetHabbo(), "", NbLot);
            foreach (Item PurchasedItem in Items)
            {
                if (Session.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    Session.SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }
        }
    }
}
