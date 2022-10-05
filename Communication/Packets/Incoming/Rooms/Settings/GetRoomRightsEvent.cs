namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.GameClients;

internal class GetRoomRightsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        var Instance = session.GetUser().CurrentRoom;
        if (Instance == null)
        {
            return;
        }

        if (!Instance.CheckRights(session))
        {
            return;
        }

        session.SendPacket(new RoomRightsListComposer(Instance));
    }
}
