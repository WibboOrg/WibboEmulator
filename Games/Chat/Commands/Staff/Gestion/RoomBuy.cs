namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomBuy : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (Room.RoomData.SellPrice == 0)
        {
            return;
        }

        if (session.GetUser().WibboPoints - Room.RoomData.SellPrice <= 0)
        {
            return;
        }

        session.GetUser().WibboPoints -= Room.RoomData.SellPrice;
        session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().WibboPoints, 0, 105));

        var ClientOwner = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(Room.RoomData.OwnerId);
        if (ClientOwner != null && ClientOwner.GetUser() != null)
        {
            ClientOwner.GetUser().WibboPoints += Room.RoomData.SellPrice;
            ClientOwner.SendPacket(new ActivityPointNotificationComposer(ClientOwner.GetUser().WibboPoints, 0, 105));
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateRemovePoints(dbClient, session.GetUser().Id, Room.RoomData.SellPrice);
            UserDao.UpdateAddPoints(dbClient, Room.RoomData.OwnerId, Room.RoomData.SellPrice);

            RoomRightDao.Delete(dbClient, Room.Id);
            RoomDao.UpdateOwner(dbClient, Room.Id, session.GetUser().Username);
            RoomDao.UpdatePrice(dbClient, Room.Id, 0);
        }

        session.SendNotification(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roombuy.sucess", session.Langue), Room.RoomData.SellPrice));

        Room.RoomData.SellPrice = 0;

        var UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

        foreach (var User in UsersToReturn)
        {
            if (User == null || User.GetClient() == null)
            {
                continue;
            }

            User.GetClient().SendPacket(new RoomForwardComposer(Room.Id));
        }
    }
}
