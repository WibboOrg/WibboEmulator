namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetGiftWrappingConfigurationEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet) => session.SendPacket(new GiftWrappingConfigurationComposer());
}