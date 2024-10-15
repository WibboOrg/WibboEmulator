namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Games.GameClients;

internal sealed class CanCreateRoomEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet) => Session.SendPacket(new CanCreateRoomComposer(false, 400));
}