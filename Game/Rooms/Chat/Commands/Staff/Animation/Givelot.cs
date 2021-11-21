using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using System.Collections.Generic;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Givelot : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Room room = Session.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }
            if (roomUserByHabbo.GetUsername() == Session.GetHabbo().Username || roomUserByHabbo.GetClient().GetHabbo().IP == Session.GetHabbo().IP)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.givelot.error", Session.Langue));
                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "notallowed", "Tentative de GiveLot: " + roomUserByHabbo.GetUsername());
                return;
            }

            int NbLot = ButterflyEnvironment.GetRandomNumber(1, 3);
            if (roomUserByHabbo.GetClient().GetHabbo().Rank > 1)
            {
                NbLot = ButterflyEnvironment.GetRandomNumber(3, 5);
            }

            int NbLotDeluxe = ButterflyEnvironment.GetRandomNumber(1, 4);
            if (roomUserByHabbo.GetClient().GetHabbo().Rank > 1)
            {
                NbLotDeluxe = ButterflyEnvironment.GetRandomNumber(3, 4);
            }

            int NbBadge = ButterflyEnvironment.GetRandomNumber(1, 2);
            if (roomUserByHabbo.GetClient().GetHabbo().Rank > 1)
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

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, roomUserByHabbo.GetClient().GetHabbo(), "", NbLot);
            Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataBadge, roomUserByHabbo.GetClient().GetHabbo(), "", NbBadge));
            if (NbLotDeluxe == 4)
            {
                Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataDeluxe, roomUserByHabbo.GetClient().GetHabbo(), "", 1));
            }

            foreach (Item PurchasedItem in Items)
            {
                if (roomUserByHabbo.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    roomUserByHabbo.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }

            string DeluxeMessage = (NbLotDeluxe == 4) ? " Et une RareBox Deluxe !" : "";
            roomUserByHabbo.GetClient().SendNotification(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", roomUserByHabbo.GetClient().Langue), NbLot, NbBadge) + DeluxeMessage);
            UserRoom.SendWhisperChat(roomUserByHabbo.GetUsername() + " à reçu " + NbLot + " RareBox et " + NbBadge + " BadgeBox!" + DeluxeMessage);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateAddGamePoints(dbClient, roomUserByHabbo.GetClient().GetHabbo().Id);
            }

            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByHabbo.GetClient(), "ACH_Extrabox", 1);
        }
    }
}
