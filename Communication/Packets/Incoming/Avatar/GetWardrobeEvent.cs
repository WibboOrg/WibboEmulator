namespace WibboEmulator.Communication.Packets.Incoming.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Games.GameClients;

internal sealed class GetWardrobeEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet) => Session.SendPacket(new WardrobeComposer(Session.User.WardrobeComponent.Wardrobes));
}
