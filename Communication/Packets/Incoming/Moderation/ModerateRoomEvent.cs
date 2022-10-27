namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal class ModerateRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.HasPermission("perm_mod"))
        {
            return;
        }

        var roomId = packet.PopInt();
        var lockRoom = packet.PopInt() == 1;
        var inappropriateRoom = packet.PopInt() == 1;
        var kickUsers = packet.PopInt() == 1;

        ModerationManager.PerformRoomAction(session, roomId, kickUsers, lockRoom, inappropriateRoom);
    }
}
