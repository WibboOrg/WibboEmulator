namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class GetGroupInfoEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var newWindow = packet.PopBoolean();

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        Session.SendPacket(new GroupInfoComposer(group, Session, newWindow));
    }
}
