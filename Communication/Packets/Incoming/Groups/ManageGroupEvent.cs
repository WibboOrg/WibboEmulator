namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;

internal class ManageGroupEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != session.GetUser().Id && !session.GetUser().HasPermission("perm_owner_all_rooms"))
        {
            return;
        }

        session.SendPacket(new ManageGroupComposer(group, group.Badge.Replace("b", "").Split('s')));
    }
}
