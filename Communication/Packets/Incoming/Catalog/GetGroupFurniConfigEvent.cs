namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class GetGroupFurniConfigEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet) => Session.SendPacket(new GroupFurniConfigComposer(GroupManager.GetGroupsForUser(Session.User.MyGroups)));
}
