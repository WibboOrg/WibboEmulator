namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class UpdateGroupBadgeEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != session.User.Id)
        {
            return;
        }

        var count = packet.PopInt();

        var badge = "";
        for (var i = 0; i < count; i++)
        {
            badge += BadgePartUtility.WorkBadgeParts(i == 0, packet.PopInt().ToString(), packet.PopInt().ToString(), packet.PopInt().ToString());
        }

        group.Badge = string.IsNullOrWhiteSpace(badge) ? "b05114s06114" : badge;

        using (var dbClient = DatabaseManager.Connection)
        {
            GuildDao.UpdateBadge(dbClient, group.Id, group.Badge);
        }

        session.SendPacket(new GroupInfoComposer(group, session));
    }
}
