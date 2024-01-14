namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RoomBuy : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (room.RoomData.SellPrice == 0)
        {
            return;
        }

        if (session.User.WibboPoints - room.RoomData.SellPrice <= 0)
        {
            return;
        }

        session.User.WibboPoints -= room.RoomData.SellPrice;
        session.SendPacket(new ActivityPointNotificationComposer(session.User.WibboPoints, 0, 105));

        var clientOwner = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(room.RoomData.OwnerId);
        if (clientOwner != null && clientOwner.User != null)
        {
            clientOwner.User.WibboPoints += room.RoomData.SellPrice;
            clientOwner.SendPacket(new ActivityPointNotificationComposer(clientOwner.User.WibboPoints, 0, 105));
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            UserDao.UpdateRemovePoints(dbClient, session.User.Id, room.RoomData.SellPrice);
            UserDao.UpdateAddPoints(dbClient, room.RoomData.OwnerId, room.RoomData.SellPrice);

            RoomRightDao.Delete(dbClient, room.Id);
            RoomDao.UpdateOwner(dbClient, room.Id, session.User.Username);
            RoomDao.UpdatePrice(dbClient, room.Id, 0);
        }

        userRoom.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("roombuy.sucess", session.Langue), room.RoomData.SellPrice));

        room.RoomData.SellPrice = 0;

        var usersToReturn = room.RoomUserManager.GetRoomUsers().ToList();
        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        foreach (var user in usersToReturn)
        {
            if (user == null || user.Client == null)
            {
                continue;
            }

            user.Client.SendPacket(new RoomForwardComposer(room.Id));
        }
    }
}
