namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;

internal class GetGuestRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var roomID = Packet.PopInt();

        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomID);
        if (roomData == null)
        {
            return;
        }

        var isLoading = Packet.PopInt() == 1;
        var checkEntry = Packet.PopInt() == 1;

        session.SendPacket(new GetGuestRoomResultComposer(session, roomData, isLoading, checkEntry));
    }
}
