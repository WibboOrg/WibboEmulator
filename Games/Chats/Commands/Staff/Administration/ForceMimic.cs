namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceMimic : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByName(username);
        if (roomUserByUserId == null)
        {
            return;
        }

        var currentRoom = roomUserByUserId.Room;
        var clientByUsername = roomUserByUserId.Client;
        if (currentRoom == null)
        {
            return;
        }

        clientByUsername.
        User.Gender = session.User.Gender;
        clientByUsername.User.Look = session.User.Look;

        if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
        {
            return;
        }

        if (!clientByUsername.User.InRoom)
        {
            return;
        }

        clientByUsername.SendPacket(new FigureUpdateComposer(clientByUsername.User.Look, clientByUsername.User.Gender));
        clientByUsername.SendPacket(new UserChangeComposer(roomUserByUserId, true));
        currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
    }
}
