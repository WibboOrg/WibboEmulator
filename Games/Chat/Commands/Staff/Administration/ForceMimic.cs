namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceMimic : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByName(username);
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

        clientByUsername.GetUser().Gender = session.GetUser().Gender;
        clientByUsername.GetUser().Look = session.GetUser().Look;

        if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
        {
            return;
        }

        if (!clientByUsername.GetUser().InRoom)
        {
            return;
        }

        clientByUsername.SendPacket(new FigureUpdateComposer(clientByUsername.GetUser().Look, clientByUsername.GetUser().Gender));
        clientByUsername.SendPacket(new UserChangeComposer(roomUserByUserId, true));
        currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
    }
}
