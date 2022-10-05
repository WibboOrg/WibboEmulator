namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;

using WibboEmulator.Games.GameClients;

internal class NavigatorSettingsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet) => session.SendPacket(new NavigatorSettingsComposer(0, 0, 0, 0, false, 0));
}