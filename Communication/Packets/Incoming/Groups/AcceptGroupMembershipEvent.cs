namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Groups;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class AcceptGroupMembershipEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var GroupId = Packet.PopInt();
        var UserId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        if (session.GetUser().Id != Group.CreatorId && !Group.IsAdmin(session.GetUser().Id) && !session.GetUser().HasPermission("perm_delete_group_limit"))
        {
            return;
        }

        if (!Group.HasRequest(UserId))
        {
            return;
        }

        var user = WibboEnvironment.GetUserById(UserId);
        if (user == null)
        {
            return;
        }

        Group.HandleRequest(UserId, true);

        user.MyGroups.Add(Group.Id);

        session.SendPacket(new GroupMemberUpdatedComposer(GroupId, user, 4));
    }
}