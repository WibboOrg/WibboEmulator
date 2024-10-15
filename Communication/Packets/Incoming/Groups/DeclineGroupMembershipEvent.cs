namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class DeclineGroupMembershipEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var userId = packet.PopInt();

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (Session.User.Id != group.CreatorId && !group.IsAdmin(Session.User.Id))
        {
            return;
        }

        if (!group.HasRequest(userId))
        {
            return;
        }

        group.HandleRequest(userId, false);
        Session.SendPacket(new UnknownGroupComposer(group.Id, userId));
    }
}
