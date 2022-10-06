namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class JoinGroupEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out var Group))
        {
            return;
        }

        if (Group.IsMember(session.GetUser().Id) || Group.IsAdmin(session.GetUser().Id) || Group.HasRequest(session.GetUser().Id) && Group.GroupType == GroupType.LOCKED || Group.GroupType == GroupType.PRIVATE)
        {
            return;
        }

        if (session.GetUser().MyGroups.Count >= 50)
        {
            session.SendNotification("Oups, il semble que vous avez atteint la limite d'adhésion au groupe! Vous pouvez seulement rejoindre jusqu'à 50 groupes.");
            return;
        }

        Group.AddMember(session.GetUser().Id);

        if (Group.GroupType == GroupType.LOCKED)
        {
            session.SendPacket(new GroupInfoComposer(Group, session));
        }
        else
        {
            session.GetUser().MyGroups.Add(Group.Id);
            session.SendPacket(new GroupFurniConfigComposer(WibboEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.GetUser().MyGroups)));
            session.SendPacket(new GroupInfoComposer(Group, session));

            if (session.GetUser().CurrentRoom != null)
            {
                session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
            }
            else
            {
                session.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
            }
        }
    }
}
