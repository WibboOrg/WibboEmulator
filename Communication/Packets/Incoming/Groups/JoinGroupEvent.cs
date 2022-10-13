namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class JoinGroupEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(packet.PopInt(), out var group))
        {
            return;
        }

        if (group.IsMember(session.GetUser().Id) || group.IsAdmin(session.GetUser().Id) || (group.HasRequest(session.GetUser().Id) && group.GroupType == GroupType.Locked) || group.GroupType == GroupType.Private)
        {
            return;
        }

        if (session.GetUser().MyGroups.Count >= 50)
        {
            session.SendNotification("Oups, il semble que vous avez atteint la limite d'adhésion au groupe! Vous pouvez seulement rejoindre jusqu'à 50 groupes.");
            return;
        }

        group.AddMember(session.GetUser().Id);

        if (group.GroupType == GroupType.Locked)
        {
            session.SendPacket(new GroupInfoComposer(group, session));
        }
        else
        {
            session.GetUser().MyGroups.Add(group.Id);
            session.SendPacket(new GroupFurniConfigComposer(WibboEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.GetUser().MyGroups)));
            session.SendPacket(new GroupInfoComposer(group, session));

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
