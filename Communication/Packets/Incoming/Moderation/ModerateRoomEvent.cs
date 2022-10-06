namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

internal class ModerateRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().HasPermission("perm_mod"))
        {
            return;
        }

        var RoomId = packet.PopInt();
        var LockRoom = packet.PopInt() == 1;
        var InappropriateRoom = packet.PopInt() == 1;
        var KickUsers = packet.PopInt() == 1;

        ModerationManager.PerformRoomAction(session, RoomId, KickUsers, LockRoom, InappropriateRoom);
    }
}
