namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;

internal class GetGroupFurniConfigEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet) => session.SendPacket(new GroupFurniConfigComposer(WibboEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.GetUser().MyGroups)));
}