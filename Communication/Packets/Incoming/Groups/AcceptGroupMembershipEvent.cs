namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;

internal class AcceptGroupMembershipEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var userId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (session.User.Id != group.CreatorId && !group.IsAdmin(session.User.Id) && !session.User.HasPermission("perm_delete_group_limit"))
        {
            return;
        }

        if (!group.HasRequest(userId))
        {
            return;
        }

        var user = WibboEnvironment.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        group.HandleRequest(userId, true);

        user.MyGroups.Add(group.Id);

        session.SendPacket(new GroupMemberUpdatedComposer(groupId, user, 4));
    }
}
