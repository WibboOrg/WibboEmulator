namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Action;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class MuteUserEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        if ((room.RoomData.MuteFuse != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true))
        {
            return;
        }

        var id = packet.PopInt();

        _ = packet.PopInt();

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(id);

        var time = packet.PopInt() * 60;

        if (roomUserByUserId == null || roomUserByUserId.IsBot || room.CheckRights(roomUserByUserId.Client, true) || roomUserByUserId.Client.User.HasPermission("mod"))
        {
            return;
        }

        var expireTime = WibboEnvironment.GetUnixTimestamp() + time;

        room.AddMute(id, expireTime);
    }
}
