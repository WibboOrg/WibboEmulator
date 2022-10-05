namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal class GetRoomBannedUsersEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
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

        if (Instance.GetBans().Count > 0)
        {
            session.SendPacket(new GetRoomBannedUsersComposer(Instance));
        }
    }
}