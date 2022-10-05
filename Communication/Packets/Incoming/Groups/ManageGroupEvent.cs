namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class ManageGroupEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var GroupId = Packet.PopInt();

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        if (Group.CreatorId != session.GetUser().Id && !session.GetUser().HasPermission("perm_owner_all_rooms"))
        {
            return;
        }

        session.SendPacket(new ManageGroupComposer(Group, Group.Badge.Replace("b", "").Split('s')));
    }
}