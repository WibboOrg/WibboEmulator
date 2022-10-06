namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

internal class ModerateRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        var RoomId = Packet.PopInt();
        var LockRoom = Packet.PopInt() == 1;
        var InappropriateRoom = Packet.PopInt() == 1;
        var KickUsers = Packet.PopInt() == 1;

        ModerationManager.PerformRoomAction(session, RoomId, KickUsers, LockRoom, InappropriateRoom);
    }
}
