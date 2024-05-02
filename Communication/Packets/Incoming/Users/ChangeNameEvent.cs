namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ChangeNameEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var roomUser = room.RoomUserManager.GetRoomUserByName(session.User.Username);
        if (roomUser == null)
        {
            return;
        }

        var newUsername = packet.PopString(16);

        if (!session.User.CanChangeName && session.User.Rank == 1)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.changename.error.1", session.Language));
            return;
        }

        if (newUsername == session.User.Username)
        {
            session.SendPacket(new UpdateUsernameComposer(session.User.Username));
            return;
        }

        if (WibboEnvironment.NameAvailable(newUsername) != 1)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.changename.error.2", session.Language));
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdateOwner(dbClient, newUsername, session.User.Username);

            UserDao.UpdateName(dbClient, session.User.Id, newUsername);

            LogFlagmeDao.Insert(dbClient, session.User.Id, session.User.Username, newUsername);
        }

        _ = GameClientManager.UpdateClientUsername(session.ConnectionID, session.User.Username, newUsername);
        _ = room.RoomUserManager.UpdateClientUsername(roomUser, session.User.Username, newUsername);
        session.User.Username = newUsername;
        session.User.CanChangeName = false;

        session.SendPacket(new UpdateUsernameComposer(newUsername));
        session.SendPacket(new UserObjectComposer(session.User));

        foreach (var roomId in session.User.UsersRooms)
        {
            if (RoomManager.TryGetRoom(roomId, out var roomOwner))
            {
                roomOwner.RoomData.OwnerName = newUsername;
            }

            RoomManager.RoomDataRemove(roomId);
        }

        room.SendPacket(new UserNameChangeComposer(newUsername, roomUser.VirtualId));

        if (session.User.Id == room.RoomData.OwnerId)
        {
            room.RoomData.OwnerName = newUsername;
            room.SendPacket(new RoomInfoUpdatedComposer(room.Id));
        }
    }
}
