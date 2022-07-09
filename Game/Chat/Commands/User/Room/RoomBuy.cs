using Wibbo.Communication.Packets.Outgoing.Inventory.Purse;
using Wibbo.Communication.Packets.Outgoing.Rooms.Session;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
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

            Client ClientOwner = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(Room.RoomData.OwnerId);
            if (ClientOwner != null && ClientOwner.GetUser() != null)
            {
                ClientOwner.GetUser().WibboPoints += Room.RoomData.SellPrice;
                ClientOwner.SendPacket(new ActivityPointNotificationComposer(ClientOwner.GetUser().WibboPoints, 0, 105));
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateRemovePoints(dbClient, Session.GetUser().Id, Room.RoomData.SellPrice);
                UserDao.UpdateAddPoints(dbClient, Room.RoomData.OwnerId, Room.RoomData.SellPrice);

                RoomRightDao.Delete(dbClient, Room.Id);
                RoomDao.UpdateOwner(dbClient, Room.Id, Session.GetUser().Username);
                RoomDao.UpdatePrice(dbClient, Room.Id, 0);
            }

            Session.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roombuy.sucess", Session.Langue), Room.RoomData.SellPrice));

            Room.RoomData.SellPrice = 0;

            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
            WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

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
