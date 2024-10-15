namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;

internal sealed class GetBuddyRequestsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet) => Session.SendPacket(new BuddyRequestsComposer(Session.User.Messenger.Requests));
}
