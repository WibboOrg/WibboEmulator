namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Items;

internal class UpdateGroupColoursEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var GroupId = Packet.PopInt();
        var Colour1 = Packet.PopInt();
        var Colour2 = Packet.PopInt();

        if (Colour1 is < 0 or > 200)
        {
            return;
        }

        if (Colour2 is < 0 or > 200)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        if (Group.CreatorId != session.GetUser().Id)
        {
            return;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            GuildDao.UpdateColors(dbClient, Colour1, Colour2, Group.Id);
        }

        Group.Colour1 = Colour1;
        Group.Colour2 = Colour2;

        session.SendPacket(new GroupInfoComposer(Group, session));
        if (session.GetUser().CurrentRoom != null)
        {
            foreach (var Item in session.GetUser().CurrentRoom.GetRoomItemHandler().GetFloor.ToList())
            {
                if (Item == null || Item.GetBaseItem() == null)
                {
                    continue;
                }

                if (Item.GetBaseItem().InteractionType is not InteractionType.GUILD_ITEM and not InteractionType.GUILD_GATE)
                {
                    continue;
                }

                session.GetUser().CurrentRoom.SendPacket(new ObjectUpdateComposer(Item, session.GetUser().CurrentRoom.RoomData.OwnerId));
            }
        }
    }
}
