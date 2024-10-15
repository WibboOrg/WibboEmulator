namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class ManageGroupEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var groupId = packet.PopInt();

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != Session.User.Id && !Session.User.HasPermission("owner_all_rooms"))
        {
            return;
        }

        Session.SendPacket(new ManageGroupComposer(group, group.Badge.Replace("b", "").Split('s')));
    }
}
