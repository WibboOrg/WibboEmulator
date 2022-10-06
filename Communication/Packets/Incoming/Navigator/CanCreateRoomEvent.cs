namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Games.GameClients;

internal class CanCreateRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet) => session.SendPacket(new CanCreateRoomComposer(false, 200));
}