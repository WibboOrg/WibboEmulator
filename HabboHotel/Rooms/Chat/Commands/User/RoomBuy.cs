using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class RoomBuy : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (Room.RoomData.SellPrice == 0)
            {
                return;
            }

            if(Session.GetHabbo().WibboPoints - Room.RoomData.SellPrice <= 0)
            {
                return;
            }

            Session.GetHabbo().WibboPoints -= Room.RoomData.SellPrice;
            Session.SendPacket(new ActivityPointsComposer(Session.GetHabbo().WibboPoints));

            GameClient ClientOwner = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Room.RoomData.OwnerId);
            if (ClientOwner != null && ClientOwner.GetHabbo() != null)
            {
                ClientOwner.GetHabbo().WibboPoints += Room.RoomData.SellPrice;
                ClientOwner.SendPacket(new ActivityPointsComposer(ClientOwner.GetHabbo().WibboPoints));
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateRemovePoints(dbClient, Session.GetHabbo().Id, Room.RoomData.SellPrice);
                UserDao.UpdateAddPoints(dbClient, Room.RoomData.OwnerId, Room.RoomData.SellPrice);

                RoomRightDao.Delete(dbClient, Room.Id);
                RoomDao.UpdateOwner(dbClient, Room.Id, Session.GetHabbo().Username);
                RoomDao.UpdatePrice(dbClient, Room.Id, 0);
            }

            Session.SendNotification(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("roombuy.sucess", Session.Langue), Room.RoomData.SellPrice));

            Room.RoomData.SellPrice = 0;

            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                {
                    continue;
                }

                User.GetClient().SendPacket(new RoomForwardComposer(Room.Id));
            }
        }
    }
}
