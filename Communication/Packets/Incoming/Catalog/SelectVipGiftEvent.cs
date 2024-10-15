namespace WibboEmulator.Communication.Packets.Incoming.Catalog;

using WibboEmulator.Games.GameClients;

internal sealed class SelectVipGiftEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var furniName = packet.PopString();


    }
}
