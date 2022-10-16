namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;

internal class KickUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if (room.RoomData.WhoCanKick != 2 && (room.RoomData.WhoCanKick != 1 || !room.CheckRights(session)) && !room.CheckRights(session, true) && session.User.Rank < 6)
        {
            return;
        }

        var pId = packet.PopInt();

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(pId);
        if (roomUserByUserId == null || roomUserByUserId.IsBot || room.CheckRights(roomUserByUserId.Client, true) || roomUserByUserId.Client.User.HasPermission("perm_mod") || roomUserByUserId.Client.User.HasPermission("perm_no_kick"))
        {
            return;
        }

        room.RoomUserManager.RemoveUserFromRoom(roomUserByUserId.Client, true, true);
    }
}