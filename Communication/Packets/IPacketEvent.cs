namespace WibboEmulator.Communication.Packets;
using WibboEmulator.Communication.Packets.Incoming;
using WibboEmulator.Games.GameClients;

public interface IPacketEvent
{
    void Parse(GameClient Session, ClientPacket packet);

    double Delay { get; }
}
