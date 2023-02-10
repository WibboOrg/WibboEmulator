namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class JoinGroupEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(packet.PopInt(), out var group))
        {
            return;
        }

        if (group.IsMember(session.User.Id) || group.IsAdmin(session.User.Id) || (group.HasRequest(session.User.Id) && group.GroupType == GroupType.Locked) || group.GroupType == GroupType.Private)
        {
            return;
        }

        if (session.User.MyGroups.Count >= 50)
        {
            session.SendNotification("Oups, il semble que vous avez atteint la limite d'adhésion au groupe! Vous pouvez seulement rejoindre jusqu'à 50 groupes.");
            return;
        }

        group.AddMember(session.User.Id);

        if (group.GroupType == GroupType.Locked)
        {
            session.SendPacket(new GroupInfoComposer(group, session));
        }
        else
        {
            session.User.MyGroups.Add(group.Id);
            session.SendPacket(new GroupFurniConfigComposer(WibboEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.User.MyGroups)));
            session.SendPacket(new GroupInfoComposer(group, session));

            var room = session.User.CurrentRoom;
            if (room != null)
            {
                room.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
            }
            else
            {
                session.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
            }
        }
    }
}
