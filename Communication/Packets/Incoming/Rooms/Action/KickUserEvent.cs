namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class KickUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (room.RoomData.WhoCanKick != 2 && (room.RoomData.WhoCanKick != 1 || !room.CheckRights(session)) && !room.CheckRights(session, true) && session.User.Rank < 6)
        {
            return;
        }

        var pId = packet.PopInt();

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(pId);
        if (roomUserByUserId == null || roomUserByUserId.IsBot || room.CheckRights(roomUserByUserId.Client, true) || roomUserByUserId.Client.User.HasPermission("mod") || roomUserByUserId.Client.User.HasPermission("no_kick"))
        {
            return;
        }

        room.RoomUserManager.RemoveUserFromRoom(roomUserByUserId.Client, true, true);
    }
}
