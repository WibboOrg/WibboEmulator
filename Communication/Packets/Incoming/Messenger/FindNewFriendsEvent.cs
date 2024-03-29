namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;

internal sealed class FindNewFriendsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet) => session.SendPacket(new RoomForwardComposer(447654));
}
