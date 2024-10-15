namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;

internal sealed class UpdateGroupColoursEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var colour1 = packet.PopInt();
        var colour2 = packet.PopInt();

        if (colour1 is < 0 or > 200)
        {
            return;
        }

        if (colour2 is < 0 or > 200)
        {
            return;
        }

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != session.User.Id)
        {
            return;
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            GuildDao.UpdateColors(dbClient, colour1, colour2, group.Id);
        }

        group.Colour1 = colour1;
        group.Colour2 = colour2;

        session.SendPacket(new GroupInfoComposer(group, session));
        if (session.User.Room != null)
        {
            foreach (var item in session.User.Room.RoomItemHandling.FloorItems.ToList())
            {
                if (item == null || item.ItemData == null)
                {
                    continue;
                }

                if (item.ItemData.InteractionType is not InteractionType.GUILD_ITEM and not InteractionType.GUILD_GATE)
                {
                    continue;
                }

                item.UpdateState(false);
            }
        }
    }
}
