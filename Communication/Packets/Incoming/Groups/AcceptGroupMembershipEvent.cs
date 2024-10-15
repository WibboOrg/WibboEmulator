namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Users;

internal sealed class AcceptGroupMembershipEvent : IPacketEvent
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

        if (Session.User.Id != group.CreatorId && !group.IsAdmin(Session.User.Id) && !Session.User.HasPermission("delete_group_limit"))
        {
            return;
        }

        if (!group.HasRequest(userId))
        {
            return;
        }

        var user = UserManager.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        group.HandleRequest(userId, true);

        user.MyGroups.Add(group.Id);

        Session.SendPacket(new GroupMemberUpdatedComposer(groupId, user, 4));
    }
}
