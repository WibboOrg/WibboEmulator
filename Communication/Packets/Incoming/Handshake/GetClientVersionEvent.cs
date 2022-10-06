namespace WibboEmulator.Communication.Packets.Incoming.Handshake;
using WibboEmulator.Games.GameClients;

internal class GetClientVersionEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var Release = Packet.PopString();
        var Type = Packet.PopString();
        var Platform = Packet.PopInt();
        var Category = Packet.PopInt();

        if (Release != "PRODUCTION-201611291003-338511768")
        {
            return;
        }

        if (Type != "FLASH" || Platform != 1 || Category != 0)
        {
            return;
        }
    }
}
