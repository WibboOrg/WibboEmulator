namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceTransfStop : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByName(username);
        if (roomUserByUserId == null)
        {
            return;
        }

        if (roomUserByUserId.IsTransf && !roomUserByUserId.IsSpectator)
        {
            var RoomClient = roomUserByUserId.Room;
            if (RoomClient != null)
            {
                roomUserByUserId.IsTransf = false;

                RoomClient.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
                RoomClient.SendPacket(new UsersComposer(roomUserByUserId));
            }
        }
    }
}
