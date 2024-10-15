namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class JoinGroupEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        if (!GroupManager.TryGetGroup(packet.PopInt(), out var group))
        {
            return;
        }

        if (group.IsMember(Session.User.Id) || group.IsAdmin(Session.User.Id) || (group.HasRequest(Session.User.Id) && group.GroupType == GroupType.Locked) || group.GroupType == GroupType.Private)
        {
            return;
        }

        if (Session.User.MyGroups.Count >= 50)
        {
            Session.SendNotification("Oups, il semble que vous avez atteint la limite d'adhésion au groupe! Vous pouvez seulement rejoindre jusqu'à 50 groupes.");
            return;
        }

        group.AddMember(Session.User.Id);

        if (group.GroupType == GroupType.Locked)
        {
            Session.SendPacket(new GroupInfoComposer(group, Session));
        }
        else
        {
            Session.User.MyGroups.Add(group.Id);
            Session.SendPacket(new GroupFurniConfigComposer(GroupManager.GetGroupsForUser(Session.User.MyGroups)));
            Session.SendPacket(new GroupInfoComposer(group, Session));

            var room = Session.User.Room;
            if (room != null)
            {
                room.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
            }
            else
            {
                Session.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
            }
        }
    }
}
