namespace WibboEmulator.Communication.Packets.Incoming.Camera;
using WibboEmulator.Games.GameClients;

internal sealed class PublishPhotoEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {

    }
}
