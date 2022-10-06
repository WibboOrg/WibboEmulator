namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal class UnbanUserFromRoomEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        var instance = session.GetUser().CurrentRoom;
        if (instance == null || !instance.CheckRights(session, true))
        {
            return;
        }

        var userId = packet.PopInt();
        var roomId = packet.PopInt();

        if (!instance.HasBanExpired(userId))
        {
            instance.RemoveBan(userId);

            session.SendPacket(new UnbanUserFromRoomComposer(roomId, userId));
        }
    }
}
