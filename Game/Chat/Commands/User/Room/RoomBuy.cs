using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RoomBuy : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (Room.RoomData.SellPrice == 0)
            {
                return;
            }

            if (Session.GetUser().WibboPoints - Room.RoomData.SellPrice <= 0)
            {
                return;
            }

            Session.GetUser().WibboPoints -= Room.RoomData.SellPrice;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().WibboPoints, 0, 105));

            Client ClientOwner = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Room.RoomData.OwnerId);
            if (ClientOwner != null && ClientOwner.GetUser() != null)
            {
                ClientOwner.GetUser().WibboPoints += Room.RoomData.SellPrice;
                ClientOwner.SendPacket(new ActivityPointNotificationComposer(ClientOwner.GetUser().WibboPoints, 0, 105));
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateRemovePoints(dbClient, Session.GetUser().Id, Room.RoomData.SellPrice);
                UserDao.UpdateAddPoints(dbClient, Room.RoomData.OwnerId, Room.RoomData.SellPrice);

                RoomRightDao.Delete(dbClient, Room.Id);
                RoomDao.UpdateOwner(dbClient, Room.Id, Session.GetUser().Username);
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
