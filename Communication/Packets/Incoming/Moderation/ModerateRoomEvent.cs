namespace WibboEmulator.Communication.Packets.Incoming.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class ModerateRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (!Session.User.HasPermission("mod"))
        {
            return;
        }

        var roomId = packet.PopInt();
        var lockRoom = packet.PopInt() == 1;
        var inappropriateRoom = packet.PopInt() == 1;
        var kickUsers = packet.PopInt() == 1;

        ModerationManager.PerformRoomAction(Session, roomId, kickUsers, lockRoom, inappropriateRoom);
    }
}
