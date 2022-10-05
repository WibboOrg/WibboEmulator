namespace WibboEmulator.Communication.Packets.Incoming.Structure;
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

        var Instance = session.GetUser().CurrentRoom;
        if (Instance == null || !Instance.CheckRights(session, true))
        {
            return;
        }

        var UserId = packet.PopInt();
        var RoomId = packet.PopInt();

        if (!Instance.HasBanExpired(UserId))
        {
            Instance.RemoveBan(UserId);

            session.SendPacket(new UnbanUserFromRoomComposer(RoomId, UserId));
        }
    }
}