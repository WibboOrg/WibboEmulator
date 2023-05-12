namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class ChangeNameEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var roomUser = room.RoomUserManager.GetRoomUserByName(session.User.Username);
        if (roomUser == null)
        {
            return;
        }

        var newUsername = packet.PopString();

        if (!session.User.CanChangeName && session.User.Rank == 1)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.1", session.Langue));
            return;
        }

        if (newUsername == session.User.Username)
        {
            session.SendPacket(new UpdateUsernameComposer(session.User.Username));
            return;
        }

        if (NameAvailable(newUsername) != 1)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.changename.error.2", session.Langue));
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateOwner(dbClient, newUsername, session.User.Username);

            UserDao.UpdateName(dbClient, session.User.Id, newUsername);

            LogFlagmeDao.Insert(dbClient, session.User.Id, session.User.Username, newUsername);
        }

        _ = WibboEnvironment.GetGame().GetGameClientManager().UpdateClientUsername(session.ConnectionID, session.User.Username, newUsername);
        _ = room.RoomUserManager.UpdateClientUsername(roomUser, session.User.Username, newUsername);
        session.User.Username = newUsername;
        session.User.CanChangeName = false;

        session.SendPacket(new UpdateUsernameComposer(newUsername));
        session.SendPacket(new UserObjectComposer(session.User));

        foreach (var roomId in session.User.UsersRooms)
        {
            if (WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var roomOwner))
            {
                roomOwner.RoomData.OwnerName = newUsername;
            }

            WibboEnvironment.GetGame().GetRoomManager().RoomDataRemove(roomId);
        }

        room.SendPacket(new UserNameChangeComposer(newUsername, roomUser.VirtualId));

        if (session.User.Id == room.RoomData.OwnerId)
        {
            room.RoomData.OwnerName = newUsername;
            room.SendPacket(new RoomInfoUpdatedComposer(room.Id));
        }
    }

    private static int NameAvailable(string username)
    {
        username = username.ToLower();

        if (username.Length > 15)
        {
            return -2;
        }

        if (username.Length < 3)
        {
            return -2;
        }

        if (!WibboEnvironment.IsValidAlphaNumeric(username))
        {
            return -1;
        }

        return WibboEnvironment.UsernameExists(username) ? 0 : 1;
    }
}
