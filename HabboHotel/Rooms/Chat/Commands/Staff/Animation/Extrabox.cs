using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using System.Collections.Generic;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class extrabox : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
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
