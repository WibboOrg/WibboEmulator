namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class UpdateGroupIdentityEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var name = WordFilterManager.CheckMessage(packet.PopString());
        var desc = WordFilterManager.CheckMessage(packet.PopString());

        if (name.Length > 50)
        {
            return;
        }

        if (desc.Length > 255)
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
            GuildDao.UpdateNameAndDesc(dbClient, groupId, name, desc);
        }

        group.Name = name;
        group.Description = desc;

        session.SendPacket(new GroupInfoComposer(group, session));

    }
}
