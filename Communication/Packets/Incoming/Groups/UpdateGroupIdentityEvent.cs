namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class UpdateGroupIdentityEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var GroupId = Packet.PopInt();
        var Name = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());
        var Desc = WibboEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(Packet.PopString());

        if (Name.Length > 50)
        {
            return;
        }

        if (Desc.Length > 255)
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
            GuildDao.UpdateNameAndDesc(dbClient, GroupId, Name, Desc);
        }

        Group.Name = Name;
        Group.Description = Desc;

        session.SendPacket(new GroupInfoComposer(Group, session));

    }
}
