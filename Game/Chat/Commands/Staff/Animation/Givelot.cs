using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Collections.Generic;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class GiveLot : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Room room = Session.GetUser().CurrentRoom;
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }
            if (roomUserByUserId.GetUsername() == Session.GetUser().Username || roomUserByUserId.GetClient().GetUser().IP == Session.GetUser().IP)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.givelot.error", Session.Langue));
                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, 0, string.Empty, "notallowed", "Tentative de GiveLot: " + roomUserByUserId.GetUsername());
                return;
            }

            int NbLot = ButterflyEnvironment.GetRandomNumber(1, 3);
            if (roomUserByUserId.GetClient().GetUser().Rank > 1)
            {
                NbLot = ButterflyEnvironment.GetRandomNumber(3, 5);
            }

            int NbLotDeluxe = ButterflyEnvironment.GetRandomNumber(1, 4);
            if (roomUserByUserId.GetClient().GetUser().Rank > 1)
            {
                NbLotDeluxe = ButterflyEnvironment.GetRandomNumber(3, 4);
            }

            int NbBadge = ButterflyEnvironment.GetRandomNumber(1, 2);
            if (roomUserByUserId.GetClient().GetUser().Rank > 1)
            {
                NbBadge = ButterflyEnvironment.GetRandomNumber(2, 3);
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(12018410, out ItemData ItemData))
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(91947063, out ItemData ItemDataBadge))
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetItemManager().GetItem(618784, out ItemData ItemDataDeluxe))
            {
                return;
            }

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, roomUserByUserId.GetClient().GetUser(), "", NbLot);
            Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataBadge, roomUserByUserId.GetClient().GetUser(), "", NbBadge));
            if (NbLotDeluxe == 4)
            {
                Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataDeluxe, roomUserByUserId.GetClient().GetUser(), "", 1));
            }

            foreach (Item PurchasedItem in Items)
            {
                if (roomUserByUserId.GetClient().GetUser().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    roomUserByUserId.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }

            string DeluxeMessage = (NbLotDeluxe == 4) ? " Et une RareBox Deluxe !" : "";
            roomUserByUserId.GetClient().SendNotification(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", roomUserByUserId.GetClient().Langue), NbLot, NbBadge) + DeluxeMessage);
            Session.SendWhisper(roomUserByUserId.GetUsername() + " à reçu " + NbLot + " RareBox et " + NbBadge + " BadgeBox!" + DeluxeMessage);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateAddGamePoints(dbClient, roomUserByUserId.GetClient().GetUser().Id);
            }

            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByUserId.GetClient(), "ACH_Extrabox", 1);
        }
    }
}
