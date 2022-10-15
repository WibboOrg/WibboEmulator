namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class UpdateGroupColoursEvent : IPacketEvent
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

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        if (group.CreatorId != session.User.Id)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            GuildDao.UpdateColors(dbClient, colour1, colour2, group.Id);
        }

        group.Colour1 = colour1;
        group.Colour2 = colour2;

        session.SendPacket(new GroupInfoComposer(group, session));
        if (session.User.CurrentRoom != null)
        {
            foreach (var item in session.User.CurrentRoom.RoomItemHandling.GetFloor.ToList())
            {
                if (item == null || item.GetBaseItem() == null)
                {
                    continue;
                }

                if (item.GetBaseItem().InteractionType is not InteractionType.GUILD_ITEM and not InteractionType.GUILD_GATE)
                {
                    continue;
                }

                session.
                User.CurrentRoom.SendPacket(new ObjectUpdateComposer(item, session.User.CurrentRoom.RoomData.OwnerId));
            }
        }
    }
}
