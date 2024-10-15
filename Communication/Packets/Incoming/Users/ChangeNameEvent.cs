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
using WibboEmulator.Games.Users;

internal sealed class ChangeNameEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var roomUser = room.RoomUserManager.GetRoomUserByName(Session.User.Username);
        if (roomUser == null)
        {
            return;
        }

        var newUsername = packet.PopString(16);

        if (!Session.User.CanChangeName && Session.User.Rank == 1)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.changename.error.1", Session.Language));
            return;
        }

        if (newUsername == Session.User.Username)
        {
            Session.SendPacket(new UpdateUsernameComposer(Session.User.Username));
            return;
        }

        if (UserManager.UsernameAvailable(newUsername) != 1)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.changename.error.2", Session.Language));
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdateOwner(dbClient, newUsername, Session.User.Username);

            UserDao.UpdateName(dbClient, Session.User.Id, newUsername);

            LogFlagmeDao.Insert(dbClient, Session.User.Id, Session.User.Username, newUsername);
        }

        _ = GameClientManager.UpdateClientUsername(Session.ConnectionID, Session.User.Username, newUsername);
        _ = room.RoomUserManager.UpdateClientUsername(roomUser, Session.User.Username, newUsername);
        Session.User.Username = newUsername;
        Session.User.CanChangeName = false;

        Session.SendPacket(new UpdateUsernameComposer(newUsername));
        Session.SendPacket(new UserObjectComposer(Session.User));

        foreach (var roomId in Session.User.UsersRooms)
        {
            if (RoomManager.TryGetRoom(roomId, out var roomOwner))
            {
                roomOwner.RoomData.OwnerName = newUsername;
            }

            RoomManager.RoomDataRemove(roomId);
        }

        room.SendPacket(new UserNameChangeComposer(newUsername, roomUser.VirtualId));

        if (Session.User.Id == room.RoomData.OwnerId)
        {
            room.RoomData.OwnerName = newUsername;
            room.SendPacket(new RoomInfoUpdatedComposer(room.Id));
        }
    }
}
