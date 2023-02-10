namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database.Daos.Guild;
using WibboEmulator.Games.GameClients;

internal sealed class UpdateGroupIdentityEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var groupId = packet.PopInt();
        var name = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
        var desc = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());

        if (name.Length > 50)
        {
            return;
        }

        if (desc.Length > 255)
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
            GuildDao.UpdateNameAndDesc(dbClient, groupId, name, desc);
        }

        group.Name = name;
        group.Description = desc;

        session.SendPacket(new GroupInfoComposer(group, session));

    }
}
